using System.Collections.Generic;
using UnityEngine;

namespace Klondike.Core
{
    public class Pool : MonoBehaviour
    {

        [SerializeField] private List<Card> availableCards = new List<Card>(); // only for showing in the inspector
        [SerializeField] private List<Card> discardedCards = new List<Card>(); // only for showing in the inspector

        private Stack<Card> coveredCards = new Stack<Card>();
        private Stack<Card> uncoveredCards = new Stack<Card>();

        // Start is called before the first frame update
        void Start()
        {
            // FOR TESTING PURPOSES ONLY
            foreach (var card in availableCards)
            {
                coveredCards.Push(card);
            }
        }

        public void AddCoveredCardToPile(Card cardToAdd)
        {
            coveredCards.Push(cardToAdd);

            availableCards.Add(cardToAdd); // only for showing in the inspector
        }

        private void TurnNextAvailableCard()
        {
            if (uncoveredCards.Count == 0 && coveredCards.Count > 0)
            {
                Card turnedCard = coveredCards.Pop();
                uncoveredCards.Push(turnedCard);
                Debug.Log(string.Format("Card {0} moved from covered to uncovered", turnedCard));
                if (coveredCards.Count > 0)
                {
                    Debug.Log("Next available card in pile: " + coveredCards.Peek());
                }

                availableCards.Remove(turnedCard); // only for showing in the inspector
                discardedCards.Add(turnedCard); // only for showing in the inspector
            }
        }

        private void OnDestroy()
        {
            coveredCards.Clear();
            uncoveredCards.Clear();
        }

        private void OnMouseDown()
        {
            TurnNextAvailableCard();
        }

    }
}