using System.Collections.Generic;
using Klondike.Core;
using Klondike.UI;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Game
{
    public class Foundation : MonoBehaviour, IValidArea
    {
        #region Serialized Fields
        [SerializeField] private CardSuit suit = CardSuit.NONE;
        [SerializeField] private List<PlayableCard> availableCards = new List<PlayableCard>(); // only for showing in the inspector 
        #endregion

        #region Attributes
        private Stack<PlayableCard> stackedCards = new Stack<PlayableCard>();

        private readonly Vector2 ANCHOR_CENTER = new Vector2(.5f, .5f);
        #endregion

        #region Properties
        public CardSuit Suit { get { return suit; } }
        public string SpotName { get { return gameObject.name; } }
        public Vector3 SpotPosition { get { return GetComponent<RectTransform>().position; } }
        public CardRank currentRank { get { return stackedCards.Count == 0 ? CardRank.NONE : stackedCards.Peek().rank; } }
        #endregion

        private void OnValidate()
        {
            GetComponentInChildren<UI_Foundation>().ChangeSuitDetails(Suit);
        }

        public void AppendCard(GameObject cardGO)
        {
            RectTransform cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.SetParent(this.transform, false);
            cardRT.anchorMin = cardRT.anchorMax = ANCHOR_CENTER;

            stackedCards.Push(cardGO.GetComponent<Card>().CardDetails);

            availableCards.Add(cardGO.GetComponent<Card>().CardDetails); // only for showing in the inspector
            GameManager.OnFoundationsUpdated?.Invoke();
        }


        /// <summary>
        /// Check if the given card can be appended to the to the Safe Spot.
        /// By the rules of the game, a card can be appended in a Foundation only
        /// if it doesn't have other cards attached, and if it has the same suit
        /// and the rank immediately after the last card rank (if the foundation is empty the rank must be ACE)
        /// </summary>
        /// <param name="cardToAppendGO">the card to append</param>
        /// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
        public bool CanAppendCard(GameObject cardToAppendGO)
        {
            bool canBeAppended;
            
            if (cardToAppendGO.GetComponentsInChildren<Card>().Length > 1)
            {
                return false;
            }

            var cardToAppend = cardToAppendGO.GetComponent<Card>().CardDetails;
            if (stackedCards.Count == 0)
            {
                canBeAppended = suit == cardToAppend.suit && cardToAppend.rank == CardRank.ACE;
            }
            else
            {
                var parentCard = stackedCards.Peek();
                canBeAppended = suit == cardToAppend.suit && (int)cardToAppend.rank == (int)parentCard.rank + 1;
            }

            Debug.Log(string.Format("Attempting to append {0} to {1} - {2}", cardToAppendGO, SpotName, canBeAppended ? "Success" : "Failed"));
            return canBeAppended;
        }

        public void DetachCard(GameObject cardGO)
        {
            var cardToRemove = stackedCards.Pop();
            availableCards.Remove(cardToRemove);
        }

    }

}