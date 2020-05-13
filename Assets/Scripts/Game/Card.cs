using System;
using System.Collections;
using System.Collections.Generic;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klondike.Game
{
    public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public static Card movingCard;

        public Action<PlayableCard> OnValuesChanged;
        public Action<bool> OnFlip;

        #region Serialized Fields
        [SerializeField] public GameObject appendSlot = default;
        [SerializeField] private PlayableCard cardDetails = default;
        [SerializeField] private bool isFaceUp = false;
        [SerializeField] private float travelTime = .4f;
        [SerializeField] private RectTransform pippo;
        [SerializeField] private Vector3 testPosition;
        Coroutine coroutineToEnd;
        #endregion

        #region Attributes
        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private Vector3 dragStartPosition;

        private IValidArea leavingSpot, landingSpot;

        private float currentClickTime, lastClickTime;
        private bool wasFaceUp; // Only for editor validation purposes
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
            GameManager.OnStartGame += ToggleInteractable;
        }

        private void OnValidate()
        {
            if (cardDetails == null) { return; }

            OnValuesChanged?.Invoke(CardDetails);
            if (isFaceUp != wasFaceUp)
            {
                wasFaceUp = isFaceUp;
                OnFlip?.Invoke(isFaceUp);
            }
        }

        public void SetCardDetails(PlayableCard card)
        {
            cardDetails = card;
            OnValuesChanged?.Invoke(CardDetails);
        }

        public void ToggleInteractable()
        {
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }

        public void Flip()
        {
            isFaceUp = !isFaceUp;
            OnFlip?.Invoke(isFaceUp);
        }

        public IEnumerator TravelTo(RectTransform destination, bool doFlip = false)
        {
            float progress = 0f;
            var startPosition = rectTransform.position;
            while (progress < 1)
            {
                progress += Time.deltaTime / travelTime;
                rectTransform.position = Vector3.Lerp(startPosition, destination.position, progress);
                yield return null;
            }
            if (doFlip)
            {
                Flip();
            }
        }

        public IEnumerator ReturnTo(Vector3 destination)
        {
            float progress = 0f;
            var startPosition = rectTransform.position;
            while (progress < 1)
            {
                progress += Time.deltaTime / travelTime;
                rectTransform.position = Vector3.Lerp(startPosition, destination, progress);
                yield return null;
            }
        }

        private IEnumerator DecideWhatToDo()
        {
            // if the card has landed on a Safe Spot, attempt to append
            if (CanPerformMove())
            {
                Debug.Log(string.Format("[Card] ended on {0}", landingSpot.SpotName));
                CreateMove();
            }
            else // if the card didn't land on a Safe Spot, or cannot append, return to position
            {
                yield return StartCoroutine(ReturnTo(dragStartPosition));
            }

            var childCanvases = GetComponentsInChildren<CanvasGroup>();
            foreach (var childCanvas in childCanvases)
            {
                childCanvas.blocksRaycasts = true;
            }

            movingCard = null;
        }

        private bool CanPerformMove()
        {
            return landingSpot != null && landingSpot.CanAppendCard(gameObject);
        }

        private void CreateMove()
        {
            GameMove gameMove = new GameMove(leavingSpot, landingSpot, gameObject);
            GameManager.Singleton.HandleNewMove(gameMove);
            //gameMove.Execute();
            //GameManager.OnValidMove?.Invoke();
        }

        public void startTravel(RectTransform destination)
        {
            StartCoroutine(TravelTo(destination));

        }

        #region Click/Drag Behaviour
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isFaceUp) { return; }

            if (movingCard != null) { return; }

            movingCard = this;

            // Save Drag Start Position for the translation animation
            dragStartPosition = rectTransform.position;
            // set the leaving spot for detaching the card
            leavingSpot = GetComponentInParent<IValidArea>();
            if (leavingSpot == null)
            {
                Debug.LogError("[Card] Detaching from unknown area");
            }

            // Disable Raycast on all childrens
            var childCanvases = GetComponentsInChildren<CanvasGroup>();
            foreach (var childCanvas in childCanvases)
            {
                childCanvas.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isFaceUp) { return; }

            if (movingCard != this) { return; }

            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isFaceUp) { return; }

            if (eventData == null) { return; } // needed only for editor issues

            if (movingCard != this) { return; }

            landingSpot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<IValidArea>();
            StartCoroutine(DecideWhatToDo());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isFaceUp) { return; }

            currentClickTime = eventData.clickTime;
            if (Mathf.Abs(currentClickTime - lastClickTime) < 0.75f)
            {
                leavingSpot = GetComponentInParent<IValidArea>();
                landingSpot = GameManager.Singleton.Hint(this);
                if (CanPerformMove())
                {
                    CreateMove();
                }
            }
            lastClickTime = currentClickTime;
        }
        #endregion

        private void OnDestroy()
        {
            GameManager.OnStartGame -= ToggleInteractable;
        }
    }
}