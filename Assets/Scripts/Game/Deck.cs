using System.Collections.Generic;
using Klondike.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.Game
{
    public class Deck : MonoBehaviour, IValidArea
    {

        #region Serialized Fields
        [SerializeField] private Image deckImage = default;
        [SerializeField] private RectTransform spawnPoint = default;
        [SerializeField] private RectTransform[] wastePile = default;
        [SerializeField] private Sprite[] deckSprites = default;
        [SerializeField] private GameObject cardPrefab = default;

        [Header("For Debugging purposes Only")]
        [SerializeField] private List<PlayableCard> availableCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private List<PlayableCard> discardedCards = new List<PlayableCard>(); // only for showing in the inspector
        #endregion

        #region Attributes
        private Stack<PlayableCard> coveredCards = new Stack<PlayableCard>();
        private LinkedList<GameObject> currentWastePile = new LinkedList<GameObject>();
        #endregion

        #region Properties
        public string SpotName { get { return gameObject.name; } }
        public RectTransform SpotPosition { get { return GetComponent<RectTransform>(); } }
        public RectTransform WastePileSlot { get { return wastePile[Mathf.Clamp(currentWastePile.Count, 0, wastePile.Length - 1)]; } }
        #endregion

        void Start()
        {
            // FOR TESTING PURPOSES ONLY - THE INITIAL LOAD WILL BE DONE BY GAME MANAGER
            foreach (var card in availableCards)
            {
                coveredCards.Push(card);
            }
        }

        /// <summary>
        /// Adds the given Card to the covered Stock Pile
        /// </summary>
        /// <param name="cardToAdd"> the Card being Added to the Stock Pile</param>
        public void AddCardToStock(PlayableCard cardToAdd)
        {
            coveredCards.Push(cardToAdd);
            availableCards.Add(cardToAdd); // only for showing in the inspector
        }

        /// <summary>
        /// If there are still cards in the Stock Pile, turns the top one and puts it in the Waste Pile.
        /// Otherwise, it flushes the Waste Pile and recreates the Stock Pile.
        /// Also changes the stock pile sprite
        /// </summary>
        public void TurnNextAvailableCard()
        {
            if (coveredCards.Count == 0)
            {
                if (currentWastePile.Count == 0) { return; }
                RecreateStock();
            }

            // turn the top available card in the deck
            PlayableCard turnedCard = coveredCards.Pop();
            AddCardToWastePile(turnedCard);
            availableCards.Remove(turnedCard); // only for showing in the inspector

            // If there are still available cards, show a covered card sprite
            deckImage.sprite = coveredCards.Count > 0 ? deckSprites[0] : deckSprites[1];

            //GameManager.OnValidMove?.Invoke();
        }

        private void RecreateStock()
        {
            var node = currentWastePile.Last;
            while (currentWastePile.Count > 0)
            {
                if (node != null)
                {
                    var nextNode = node.Previous;
                    PlayableCard cardToAdd = node.Value.GetComponent<Card>().CardDetails;
                    AddCardToStock(cardToAdd);
                    currentWastePile.Remove(node);
                    Destroy(node.Value);
                    discardedCards.Remove(cardToAdd); // only for showing in the inspector
                    node = nextNode;
                }
            }
        }

        private void AddCardToWastePile(PlayableCard turnedCard)
        {

            //Debug.Log(string.Format("[Deck] Card {0} moved from covered to uncovered", turnedCard));
            // first of all, set the last card as non interactable;
            if (currentWastePile.Last != null)
            {
                currentWastePile.Last.Value.GetComponent<Card>().ToggleInteractable();
            }

            var newCard = Instantiate(cardPrefab);
            newCard.GetComponent<Card>().SetCardDetails(turnedCard);
            newCard.gameObject.name = turnedCard.ToString();

            newCard.transform.SetParent(transform, false); // non mi serve sia figlia delle rect transform degli slot 
            newCard.GetComponent<RectTransform>().position = spawnPoint.position;
            StartCoroutine(newCard.GetComponent<Card>().TravelTo(WastePileSlot, true));

            // Shift all previously occupied slots to the left
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

            newCard.GetComponent<Card>().ToggleInteractable();
            currentWastePile.AddLast(newCard);
            discardedCards.Add(turnedCard); // only for showing in the inspector
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
            Debug.LogError("[Deck] ERROR: trying to append a card to the waste pile");
        }

        public void DetachCard(GameObject cardGO)
        {
            var cardToRemove = cardGO.GetComponent<Card>().CardDetails;
            discardedCards.Remove(cardToRemove);
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
            coveredCards.Clear();
            currentWastePile.Clear();
        }

    }
}