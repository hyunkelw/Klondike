using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.Core
{
    public class Deck : MonoBehaviour, IValidArea
    {

        #region Serialized Fields
        [SerializeField] private List<PlayableCard> availableCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private List<PlayableCard> discardedCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private Image stockImage = default;
        [SerializeField] private RectTransform wastePile = default;
        [SerializeField] private Sprite[] stockSprites = default;
        [SerializeField] private GameObject cardPrefab = default;
        #endregion

        #region Attributes
        private Stack<PlayableCard> coveredCards = new Stack<PlayableCard>();
        private Stack<PlayableCard> uncoveredCards = new Stack<PlayableCard>();
        #endregion

        #region Properties
        public string SpotName { get { return gameObject.name; } }
        public Vector2 SpotPosition { get { return GetComponent<RectTransform>().anchoredPosition; } }

        #endregion

        // Start is called before the first frame update
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
            // if there are still available cards in deck, turn the top one
            if (coveredCards.Count > 0)
            {
                PlayableCard turnedCard = coveredCards.Pop();
                AddCardToWastePile(turnedCard);
                availableCards.Remove(turnedCard); // only for showing in the inspector

                // If there are still available cards, show a covered card sprite
                if (coveredCards.Count > 0)
                {
                    //Debug.Log("Next available card in pile: " + coveredCards.Peek());
                    stockImage.sprite = stockSprites[0];
                }
                else
                {
                    stockImage.sprite = stockSprites[1];
                }
            }
            else // No more cards, recreate the pile from the waste pile
            {
                RecreateStock();
            }
            GameManager.OnValidMove?.Invoke();
        }

        private void RecreateStock()
        {
            // cycle through all cards of the waste pile, and add them back to the stock
            while (uncoveredCards.Count > 0)
            {
                PlayableCard cardToAdd = uncoveredCards.Pop();
                AddCardToStock(cardToAdd);

                discardedCards.Remove(cardToAdd); // only for showing in the inspector
            }
            FlushWastePile();
            stockImage.sprite = stockSprites[0];
        }

        private void FlushWastePile()
        {
            foreach (Transform cardTransform in wastePile.transform)
            {
                Destroy(cardTransform.gameObject);
            }
        }

        private void AddCardToWastePile(PlayableCard turnedCard)
        {
            uncoveredCards.Push(turnedCard);
            //Debug.Log(string.Format("Card {0} moved from covered to uncovered", turnedCard));
            var newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity);

            newCard.gameObject.name = turnedCard.ToString();
            newCard.transform.SetParent(wastePile, false);
            newCard.GetComponent<Card>().SetCardDetails(turnedCard);
            newCard.GetComponent<Card>().Flip();
            discardedCards.Add(turnedCard); // only for showing in the inspector
        }


        public void DetachCard(GameObject cardGO)
        {
            var cardToRemove = uncoveredCards.Pop();
            discardedCards.Remove(cardToRemove);
            // TO DO: Aggiornare la carta mostrata
        }

        public void AppendCard(GameObject cardGO)
        {
            Debug.LogError("[Deck] ERROR: trying to append a card to the waste pile");
        }

        /// <summary>
        /// Check if the given card can be appended to the Safe Spot.
        /// By the rules of the game, a card can never be appended on the waste pile.
        /// </summary>
        /// <param name="cardToAppend">the card to append</param>
        /// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
        public bool CanAppendCard(PlayableCard cardDetails)
        {
            return false;
        }

        private void OnDestroy()
        {
            coveredCards.Clear();
            uncoveredCards.Clear();
        }

    }
}