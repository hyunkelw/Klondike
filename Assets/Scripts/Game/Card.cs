using System;
using System.Collections;
using Klondike.Core;
using Klondike.UI; // serve solo per le prove in Editor
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

    public Action<PlayableCard> OnValuesChanged;

    #region Serialized Fields
    [SerializeField] public GameObject appendSlot = default;
    [SerializeField] private PlayableCard cardDetails = default;
    [SerializeField] private bool isFaceUp = false;
    [SerializeField] private float travelTime = .4f;

    #endregion

    #region Attributes
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 dragStartPosition;

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
        //startPosition = rectTransform.position;
    }

    private void OnValidate()
    {
        GetComponentInChildren<UI_CardDisplay>().ChangeCardDetails(CardDetails);
    }

    public void SetCardDetails(PlayableCard card)
    {
        cardDetails = card;
        OnValuesChanged?.Invoke(CardDetails);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isFaceUp) { return; }

        canvasGroup.alpha = .2f;
        // Save Drag Start Position for the translation animation
        dragStartPosition = rectTransform.position;
        // save the leaving spot for future usage
        leavingSpot = GetComponentInParent<IValidArea>();
        if (leavingSpot != null)
        {
            Debug.Log(string.Format("[Card] Detaching from {0}", leavingSpot.SpotName));
        }

        canvasGroup.blocksRaycasts = false;

        //appendSlot.GetComponent<CanvasGroup>().enabled = false; // disattivo il suo slot per evitare comportamenti insoliti

        // TO DO: fare in modo che la carta sia sempre sopra a tutte le altre
    }

    private IEnumerator MoveTo(Vector3 spotPosition)
    {
        float progress = 0f;
        var startPosition = rectTransform.position;
        while (progress < 1)
        {
            progress += Time.deltaTime / travelTime ;
            rectTransform.position = Vector3.Lerp(startPosition, spotPosition, progress);
            yield return null;
        }
        
        Debug.Log("Coroutine Ended");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isFaceUp) { return; }
        //rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f) / canvas.scaleFactor;
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
        if (landingSpot != null && landingSpot.CanAppendCard(gameObject))
        {
            Debug.Log(string.Format("ended on {0}", landingSpot.SpotName));
            StartCoroutine(SwitchMove());
     
        }
        else // if the card didn't land on a Safe Spot, or cannot append, return to position
        {
            StartCoroutine(MoveTo(dragStartPosition));
            //ReturnToPosition();
        }
    }

    private IEnumerator SwitchMove()
    {
        yield return StartCoroutine(MoveTo(landingSpot.SpotPosition));
        leavingSpot.DetachCard(gameObject);
        landingSpot.AppendCard(gameObject);
        GameManager.OnValidMove?.Invoke();
    }


    public void Flip()
    {
        isFaceUp = !isFaceUp;
        StartCoroutine(GetComponentInChildren<UI_CardDisplay>().Flip(isFaceUp));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isFaceUp) { return; }
        if (eventData.clickCount > 1)
        {
            GameManager.Singleton.AutoMove(this);
            Debug.Log("[Card] Double Clicked!");
        }
    }
}

