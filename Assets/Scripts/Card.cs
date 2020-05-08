using System;
using System.Collections;
using Klondike.Core;
using UnityEngine;
using UnityEngine.EventSystems;

//public class CardBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] public GameObject appendSlot = default;

    public Action<PlayableCard> OnValuesChanged;

    private PlayableCard cardDetail = default;

    public PlayableCard CardDetails { get { return cardDetail; } }

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 dragStartPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        canvas = GameObject.FindGameObjectWithTag("Game Canvas").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .2f;
        dragStartPosition = rectTransform.anchoredPosition;
        Debug.Log(string.Format("[Card] Drag begun at {0}", rectTransform.anchoredPosition));
        canvasGroup.blocksRaycasts = false;
        appendSlot.GetComponent<CanvasGroup>().enabled = false; // disattivo il suo slot per evitare comportamenti insoliti
        // TO DO: fare in modo che la carta sia sempre sopra a tutte le altre
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var landingPile = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Pile>();
        canvasGroup.blocksRaycasts = true;
        appendSlot.GetComponent<CanvasGroup>().enabled = true;
        canvasGroup.alpha = 1f;
        if (landingPile == null)
        {
            //StartCoroutine(ReturnToPosition());
            ReturnToPosition();
        }
        else
        {
            Debug.Log(string.Format("ended on {0}", landingPile.gameObject.name));
            if (PlayableCard.CanBeAppended(cardDetail, landingPile.AttachableCard)) 
            {
                landingPile.AppendCard(this.gameObject);
                GameManager.OnValidMove?.Invoke();
            }
            else
            {
                ReturnToPosition();
            }
        }
    }

    //private IEnumerator ReturnToPosition()
    //{
    //    while (rectTransform.anchoredPosition != dragStartPosition)
    //    {
    //        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, dragStartPosition, Time.deltaTime);
    //        yield return null;
    //    }

    //}

    private void ReturnToPosition()
    {
        rectTransform.anchoredPosition = dragStartPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void SetCardDetails(PlayableCard card)
    {
        cardDetail = card;
        OnValuesChanged?.Invoke(cardDetail);
    }
}

