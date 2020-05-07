using System.Collections.Generic;
using UnityEngine;

namespace Klondike.Core
{
    public class Pile : MonoBehaviour
    {

        [SerializeField] private List<Card> availableCards = new List<Card>(); // only for showing in the inspector
        [SerializeField] private List<Card> onStackCards = new List<Card>(); // only for showing in the inspector

        private Stack<Card> coveredCards = new Stack<Card>();
        private Queue<Card> uncoveredCards = new Queue<Card>();

        public int CoveredPileStartingSize { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            // FOR TESTING PURPOSES ONLY
            foreach (var card in availableCards)
            {
                coveredCards.Push(card);
            }
        }

        private void OnEnable()
        {
            //GameManager.OnValidMove += TurnNextAvailableCard;

            // FOR TESTING PURPOSES ONLY
            GameManager.OnValidMove += DestroyCard;
        }

        public void AddCoveredCardToPile(Card cardToAdd)
        {
            coveredCards.Push(cardToAdd);

            availableCards.Add(cardToAdd); // only for showing in the inspector
        }

        // Turn the card on top if there's one available.
        // TODO: this must be done automatically ONLY after a valid move.
        private void TurnNextAvailableCard()
        {
            if (uncoveredCards.Count == 0 && coveredCards.Count > 0)
            {
                Card turnedCard = coveredCards.Pop();
                uncoveredCards.Enqueue(turnedCard);
                Debug.Log(string.Format("Card {0} moved from covered to uncovered", turnedCard));
                if (coveredCards.Count > 0)
                {
                    Debug.Log("Next available card in pile: " + coveredCards.Peek());
                }

                availableCards.Remove(turnedCard); // only for showing in the inspector
                onStackCards.Add(turnedCard); // only for showing in the inspector
            }
        }


        // FOR TESTING PURPOSES ONLY
        private void DestroyCard()
        {
            Card poppedCard = uncoveredCards.Dequeue();
            Debug.Log(string.Format("Card {0} destroyed", poppedCard));
        }

        private void OnDestroy()
        {
            //GameManager.OnValidMove -= TurnNextAvailableCard;
            coveredCards.Clear();
            uncoveredCards.Clear();

            // FOR TESTING PURPOSES ONLY
            GameManager.OnValidMove -= DestroyCard;
        }

        private void OnMouseDown()
        {
            TurnNextAvailableCard();
        }

    }
}