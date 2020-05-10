using System.Collections.Generic;
using Klondike.UI;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Core
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
        public Vector2 SpotPosition { get { return GetComponent<RectTransform>().anchoredPosition; } }
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

        public bool CanAppendCard(PlayableCard cardToAppend)
        {
            bool canBeAppended;
            if (stackedCards.Count == 0)
            {
                canBeAppended = suit == cardToAppend.suit && cardToAppend.rank == CardRank.ACE;
            }
            else
            {
                var parentCard = stackedCards.Peek();
                canBeAppended = suit == cardToAppend.suit && (int)cardToAppend.rank == (int)parentCard.rank + 1;
            }

            Debug.Log(string.Format("Attempting to append {0} to {1} - {2}", cardToAppend, SpotName, canBeAppended ? "Success" : "Failed"));
            return canBeAppended;
        }

        public void DetachCard(GameObject cardGO)
        {
            var cardToRemove = stackedCards.Pop();
            availableCards.Remove(cardToRemove);
        }

    }

}