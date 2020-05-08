using System;
using UnityEngine;

namespace Klondike.Core
{

    public class GameManager : MonoBehaviour
    {
        Deck deck = new Deck();

        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Stock stock = default;

        public static Action OnValidMove;

        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log(deck);
        }

        // Update is called once per frame
        void Update()
        {
            // Diventerà il "NEW GAME"
            if (Input.GetKeyDown(KeyCode.Space))
            {
                deck.Shuffle();
                Debug.Log("Deck shuffled");
                StartGame();
                //Debug.Log(deck);
            }


            // FOR TESTING PURPOSES ONLY
            if (Input.GetMouseButtonDown(1))
            {
                OnValidMove?.Invoke();
            }
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
            while (givenCards < Deck.DECK_SIZE)
            {
                stock.AddCardToStock(deck.GetNextCard());
                givenCards++;
            }
            OnValidMove?.Invoke();
        }

    }
}
