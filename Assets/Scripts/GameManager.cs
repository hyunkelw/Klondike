using System;
using System.Collections.Generic;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Core
{

    public class GameManager : MonoSingleton<GameManager>
    {
        PlayableDeck deck = new PlayableDeck();

        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck stock = default;
        [SerializeField] private Foundation[] foundations = default;

        private int movesCounter = 0;
        private int score = 0;
        private float elapsedTime = 0;
        private List<IValidArea> spots = new List<IValidArea>();

        public static Action OnValidMove, OnStartGame, OnFoundationsUpdated;

        public int Moves { get { return movesCounter; } }

        private void Awake()
        {
            SetupSingleton();
            spots.AddRange(foundations);
            spots.AddRange(piles);
        }


        private void OnEnable()
        {
            OnValidMove += UpdateMovesCounter;
            OnFoundationsUpdated += CheckWinCondition;
        }

        private void CheckWinCondition()
        {
            foreach (var foundation in foundations)
            {
                if (foundation.currentRank != CardRank.K)
                {
                    Debug.Log("Not won yet...");
                    return;
                }
            }
            Debug.Log("Game Won..");
        }

        // Start is called before the first frame update
        void Start()
        {
            deck.Shuffle();
            StartGame();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGame()
        {
            int givenCards = 0;
            for (int i = 0; i < piles.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    //piles[i].AddCoveredCardToPile(deck.GetNextCard());
                    piles[i].AddCardToPile(deck.GetNextCard());
                    givenCards++;
                }
            }
            while (givenCards < PlayableDeck.DECK_SIZE)
            {
                stock.AddCardToStock(deck.GetNextCard());
                givenCards++;
            }
            OnStartGame?.Invoke();
        }

        private void UpdateMovesCounter()
        {
            movesCounter++;
        }

        private void OnDestroy()
        {
            OnValidMove -= UpdateMovesCounter;
        }

        public void AutoMove(Card card)
        {
            foreach (var spot in spots)
            {
                if (spot.CanAppendCard(card.gameObject) )
                {
                    Debug.Log(string.Format("[GameManager] card {0} can be moved onto {1}", card.name, spot.SpotName));
                    return;
                }
            }
        }
    }
}
