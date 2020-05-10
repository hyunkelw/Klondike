using System;
using UnityEngine;

namespace Klondike.Core
{

    public class GameManager : MonoBehaviour
    {
        PlayableDeck deck = new PlayableDeck();

        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck stock = default;

        public static Action OnValidMove;

        // Start is called before the first frame update
        void Start()
        {
            deck.Shuffle();
            StartGame();
        }

        // Update is called once per frame
        void Update()
        {

            // FOR TESTING PURPOSES ONLY
            //if (Input.GetMouseButtonDown(1))
            //{
            //    OnValidMove?.Invoke();
            //}
        }

        private void StartGame()
        {
            int givenCards = 0;
            for (int i = 0; i < piles.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    piles[i].AddCoveredCardToPile(deck.GetNextCard());
                    givenCards++;
                }
            }
            while (givenCards < PlayableDeck.DECK_SIZE)
            {
                stock.AddCardToStock(deck.GetNextCard());
                givenCards++;
            }
            OnValidMove?.Invoke();
        }

    }
}
