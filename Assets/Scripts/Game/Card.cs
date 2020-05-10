using System;
using System.Collections;
using Klondike.Core;
using Klondike.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Action<PlayableCard> OnValuesChanged;

    #region Serialized Fields
    [SerializeField] public GameObject appendSlot = default;
    [SerializeField] private PlayableCard cardDetails = default;
    [SerializeField] private bool isFaceUp = false;
    [SerializeField] private float speed = 3500f;
    
    #endregion

    #region Attributes
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 dragStartPosition;

    private IValidArea leavingSpot;
    private IValidArea landingSpot;


    #endregion

    #region Properties
    public PlayableCard CardDetails { get { return cardDetails; } }
    public bool IsFaceUp { get { return isFaceUp; } }
    #endregion

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvas = GameObject.FindGameObjectWithTag("Game Canvas").GetComponent<Canvas>();
    }

    private void OnValidate()
    {
        GetComponentInChildren<UI_CardDisplay>().ChangeCardDetails(CardDetails);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isFaceUp)        { return; }
        canvasGroup.alpha = .2f;
        // Save Drag Start Position for the translation animation
        dragStartPosition = rectTransform.anchoredPosition;
        // save the leaving spot for future usage
        leavingSpot = GetComponentInParent<IValidArea>();
        if (leavingSpot != null)
        {
            Debug.Log(string.Format("[Card] Detaching from {0}", leavingSpot.SpotName));
        }

        //Debug.Log(string.Format("[Card] Drag begun at {0}", rectTransform.anchoredPosition));
        canvasGroup.blocksRaycasts = false;

        //appendSlot.GetComponent<CanvasGroup>().enabled = false; // disattivo il suo slot per evitare comportamenti insoliti
        // TO DO: fare in modo che la carta sia sempre sopra a tutte le altre
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isFaceUp) { return; }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isFaceUp) { return; }

        if (eventData == null) { return; } // needed only for editor issues

        landingSpot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<IValidArea>();
        canvasGroup.blocksRaycasts = true;
        appendSlot.GetComponent<CanvasGroup>().enabled = true;
        canvasGroup.alpha = 1f;

        // if the card has landed on a Safe Spot, attempt to append
        if (landingSpot != null && landingSpot.CanAppendCard(CardDetails))
        {
            Debug.Log(string.Format("ended on {0}", landingSpot.SpotName));
            leavingSpot.DetachCard(gameObject);
            landingSpot.AppendCard(gameObject);
            GameManager.OnValidMove?.Invoke();
        }
        else // if the card didn't land on a Safe Spot, or cannot append, return to position
        {
            StartCoroutine(ReturnToPosition());
            //ReturnToPosition();
        }
    }

    private IEnumerator ReturnToPosition()
    {
        while (rectTransform.anchoredPosition != dragStartPosition)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, dragStartPosition, speed * Time.deltaTime);
            yield return null;
        }

    }

    public void Flip()
    {
        isFaceUp = !isFaceUp;
        StartCoroutine(GetComponentInChildren<UI_CardDisplay>().Flip(isFaceUp));
    }

    //private void ReturnToPosition()
    //{
    //    rectTransform.anchoredPosition = dragStartPosition;
    //}


    // TO DO : questo servirà per il doppio click e l'automove?
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        //Flip();
    }


    public void SetCardDetails(PlayableCard card)
    {
        cardDetails = card;
        OnValuesChanged?.Invoke(CardDetails);
    }
}

