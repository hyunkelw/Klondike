using System;
using System.Collections;
using System.Collections.Generic;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.Game
{
    public class Deck : MonoBehaviour, IValidArea
    {
        public Action<GameMove> ExecuteAction, UndoAction;

        #region Serialized Fields
        [SerializeField] private Image deckImage = default;
        [SerializeField] private RectTransform spawnPoint = default;
        [SerializeField] private RectTransform[] wastePile = default;
        [SerializeField] private Sprite[] deckSprites = default;

        [Header("For Debugging purposes Only")]
        [SerializeField] private GameObject lastCardFetched; // serialized for debugging purposes only
        [SerializeField] private List<GameObject> availableCards = new List<GameObject>(); // only for showing in the inspector
        [SerializeField] private List<GameObject> discardedCards = new List<GameObject>(); // only for showing in the inspector
        #endregion

        #region Attributes
        private LinkedList<GameObject> currentDeck = new LinkedList<GameObject>();
        private LinkedList<GameObject> currentWastePile = new LinkedList<GameObject>();
        #endregion

        #region Properties
        public string SpotName { get { return gameObject.name; } }
        public RectTransform SpotPosition { get { return GetComponent<RectTransform>(); } }
        public RectTransform WastePileSlot { get { return wastePile[Mathf.Clamp(currentWastePile.Count, 0, wastePile.Length - 1)]; } }
        public RectTransform WastePileUndoSlot { get { return wastePile[Mathf.Clamp(wastePile.Length - currentDeck.Count, 0, wastePile.Length - 1)]; } }
        public GameObject DeckTopCard { get { return currentDeck.Last.Value; } }
        public GameObject WasteBottomCard { get { return currentWastePile.Count > 0 ? currentWastePile.Last.Value : null; } }
        public Action<GameMove> Execute { get { return ExecuteAction; } }
        public Action<GameMove> Undo { get { return UndoAction; } }
        #endregion

        private void OnEnable()
        {
            ExecuteAction = ExecuteMove;
            UndoAction = UndoMove;
        }

        public void ExecuteMove(GameMove moveToExecute)
        {
            switch (moveToExecute.MoveType)
            {
                case MoveType.FETCH_CARD:
                    FetchCard();
                    break;
                case MoveType.RECYCLE_WASTE:
                    StartCoroutine(RecycleWaste());
                    break;
                case MoveType.WASTE_TO_FOUNDATION:
                    FetchCard();
                    break;
                case MoveType.WASTE_TO_TABLEAU:
                    StartCoroutine(RecycleWaste());
                    break;


                default:
                    Debug.LogError("[Deck] the move to execute doesn't belong to this object");
                    break;
            }
        }

        public void UndoMove(GameMove moveToUndo)
        {
            switch (moveToUndo.MoveType)
            {
                case MoveType.FETCH_CARD:
                    UndoFetchCard(moveToUndo.Card);
                    break;
                case MoveType.RECYCLE_WASTE:
                    StartCoroutine(UndoRecycleWaste());
                    break;
                default:
                    Debug.LogError("[Deck] the move to execute doesn't belong to this object");
                    break;
            }
        }

        /// <summary>
        /// Adds the given Card to the covered Stock Pile
        /// </summary>
        /// <param name="cardToAdd"> the Card being Added to the Stock Pile</param>
        public void AddCardToStock(GameObject cardToAdd)
        {
            availableCards.Add(cardToAdd); // only for showing in the inspector
            currentDeck.AddLast(cardToAdd);
        }

        public void ChooseButtonAction()
        {
            if (availableCards.Count == 0 && currentWastePile.Count == 0) { return; }

            GameMove gameMove;

            if (availableCards.Count == 0 && currentWastePile.Count > 0)
            {
                // create a "RecreateStock" Move
                gameMove = new GameMove(this, this, null);
            }
            else
            {
                //availableCardIndex = availableCardIndex - 1 < 0 ? availableCards.Count - 1 : availableCardIndex - 1;
                lastCardFetched = DeckTopCard; // only for showing in the inspector
                gameMove = new GameMove(this, this, DeckTopCard);
            }
            // If there are still available cards, show a covered card sprite
            deckImage.sprite = availableCards.Count > 0 ? deckSprites[0] : deckSprites[1];

            GameManager.Singleton.HandleNewMove(gameMove);
        }

        /// <summary>
        /// If there are still cards in the Stock Pile, turns the top one and puts it in the Waste Pile.
        /// Otherwise, it flushes the Waste Pile and recreates the Stock Pile.
        /// Also changes the stock pile sprite
        /// </summary>
        public void FetchCard()
        {
            // turn the top available card in the deck
            FromDeckToWaste(DeckTopCard);

            // add it to the bottom of current waste pile
            currentWastePile.AddLast(DeckTopCard);
            discardedCards.Add(DeckTopCard); // only for showing in the inspector

            // and remove it from the deck
            availableCards.Remove(DeckTopCard); // only for showing in the inspector
            currentDeck.Remove(DeckTopCard);
        }

        public void UndoFetchCard(GameObject targetCard)
        {
            // first, remove the card from the bottom of the current waste pile
            currentWastePile.Remove(targetCard);
            discardedCards.Remove(targetCard); // only for showing in the inspector

            StartCoroutine(FromWasteToDeck(targetCard));

            // add it to the top of current deck
            AddCardToStock(targetCard);

            // set the next card to undo
            lastCardFetched = WasteBottomCard;
        }

        private void FromDeckToWaste(GameObject cardToTurn)
        {
            // first of all, set the last card as non interactable, if there's one
            if (currentWastePile.Last != null)
            {
                currentWastePile.Last.Value.GetComponent<Card>().ToggleInteractable();
            }

            var cardComponent = cardToTurn.GetComponent<Card>();
            // after that, send the card to the current waste pile slot
            StartCoroutine(cardComponent.TravelTo(WastePileSlot));
            StartCoroutine(cardComponent.Flip());

            // Shift all previously occupied slots to the left if needed
            if (currentWastePile.Count > wastePile.Length - 1)
            {
                var node = currentWastePile.Last;
                for (int i = wastePile.Length - 1; i >= 1; i--)
                {
                    var cardToShift = node.Value.GetComponent<Card>();
                    StartCoroutine(cardToShift.TravelTo(wastePile[i - 1]));
                    node = node.Previous;
                }
            }

            // lastly, set the turned card as interactable
            cardComponent.ToggleInteractable();

        }

        private IEnumerator FromWasteToDeck(GameObject cardToPutBack)
        {
            // set this card as Non interactable
            var cardComponent = cardToPutBack.GetComponent<Card>();
            cardComponent.ToggleInteractable();

            // send it back to the deck position, waiting for the animation to stop
            yield return StartCoroutine(cardComponent.Flip());
            yield return StartCoroutine(cardComponent.TravelTo(spawnPoint));

            // shift all remaining cards to the right if needed
            if (currentWastePile.Count > wastePile.Length - 1)
            {
                var node = currentWastePile.Last;
                for (int i = wastePile.Length - 1; i >= 1; i--)
                {
                    var cardToShift = node.Value.GetComponent<Card>();
                    StartCoroutine(cardToShift.TravelTo(wastePile[i]));
                    node = node.Previous;
                }
            }

            // lastly, set the now last card as interactable, if there's one;
            if (currentWastePile.Last != null)
            {
                currentWastePile.Last.Value.GetComponent<Card>().ToggleInteractable();
            }
        }

        private IEnumerator RecycleWaste()
        {
            // cycle through all the current waste pile
            var node = currentWastePile.Last;
            while (currentWastePile.Count > 0)
            {
                // for each card
                if (node != null)
                {
                    var nextNode = node.Previous;
                    var cardToAdd = node.Value;

                    //add it back to the available cards
                    AddCardToStock(cardToAdd);
                    var cardComponent = cardToAdd.GetComponent<Card>();

                    // send it back to the deck position (without waiting for the animation to complete)
                    StartCoroutine(cardComponent.Flip());
                    StartCoroutine(cardComponent.TravelTo(spawnPoint));

                    currentWastePile.Remove(node);
                    discardedCards.Remove(cardToAdd); // only for showing in the inspector
                    node = nextNode;
                }
            }
            yield return null;
        }

        private IEnumerator UndoRecycleWaste()
        {
            // cycle through all the current available pile
            var node = currentDeck.Last;
            while (currentDeck.Count > 0)
            {
                // for each card
                if (node != null)
                {
                    var nextNode = node.Previous;
                    var cardToAdd = node.Value;
                    var cardComponent = cardToAdd.GetComponent<Card>();

                    // send it  back to the waste pile position (without waiting for the animation to complete)
                    StartCoroutine(cardComponent.Flip());
                    StartCoroutine(cardComponent.TravelTo(WastePileUndoSlot));

                    // add it back to the waste pile,
                    currentWastePile.AddLast(cardToAdd);
                    discardedCards.Add(cardToAdd); // only for showing in the inspector

                    currentDeck.Remove(node);
                    availableCards.Remove(cardToAdd); // only for showing in the inspector
                    node = nextNode;
                }
            }
            yield return null;
        }

        /// <summary>
        /// Check if the given card can be appended to the Safe Spot.
        /// By the rules of the game, a card can never be appended on the waste pile.
        /// </summary>
        /// <param name="cardToAppend">the card to append</param>
        /// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
        public bool CanAppendCard(GameObject cardToAppend)
        {
            return false;
        }

        public void AppendCard(GameObject cardGO)
        {
            var siblingIndex = 1;
            // if there are other cards in the waste pile, take the bottom one, set it as non interactable and get its sibling index
            if (WasteBottomCard != null)
            {
                siblingIndex = WasteBottomCard.GetComponent<RectTransform>().GetSiblingIndex();
                WasteBottomCard.GetComponent<Card>().ToggleInteractable();
            }
            var cardComponent = cardGO.GetComponent<Card>();

            StartCoroutine(cardComponent.TravelTo(WastePileSlot));
            cardGO.transform.SetParent(GetComponent<RectTransform>(), true);
            cardGO.transform.SetSiblingIndex(siblingIndex + 1);

            // Shift all previously occupied slots to the left if needed
            if (currentWastePile.Count > wastePile.Length - 1)
            {
                var node = currentWastePile.Last;
                for (int i = wastePile.Length - 1; i >= 1; i--)
                {
                    var cardToShift = node.Value.GetComponent<Card>();
                    StartCoroutine(cardToShift.TravelTo(wastePile[i - 1]));
                    node = node.Previous;
                }
            }
            currentWastePile.AddLast(cardGO);

            //Debug.LogError("[Deck] ERROR: trying to append a card to the waste pile");
        }

        public void DetachCard(GameObject cardGO)
        {
            currentWastePile.Remove(cardGO);

            // if there are no cards left, do nothing more
            if (currentWastePile.Count == 0) { return; }

            // make the last (after the removal) card as interactable
            var nextCard = currentWastePile.Last.Value.GetComponent<Card>();
            nextCard.ToggleInteractable();

            // if the cards left in the waste pile are more than the available waste slots, shift right
            if (currentWastePile.Count > wastePile.Length - 1)
            {
                var node = currentWastePile.Last;
                for (int i = wastePile.Length - 1; i >= 1; i--)
                {
                    var cardToShift = node.Value.GetComponent<Card>();
                    StartCoroutine(cardToShift.TravelTo(wastePile[i]));
                    node = node.Previous;
                }
            }
        }

        private void OnDestroy()
        {
            //coveredCards.Clear();
            currentWastePile.Clear();
            ExecuteAction = null;
            UndoAction = null;
        }

    }
}