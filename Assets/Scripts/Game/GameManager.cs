using System;
using System.Collections.Generic;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Game
{

    public class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnValidMove, OnStartGame, OnFoundationsUpdated;

        #region Serialized Fields
        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck stock = default;
        [SerializeField] private Foundation[] foundations = default;
        #endregion

        #region Attributes
        private PlayableDeck deck = new PlayableDeck();
        private int movesCounter = 0;
        private int score = 0;
        private List<IValidArea> spots = new List<IValidArea>();
        #endregion

        #region Properties
        public int Moves { get { return movesCounter; } } 
        #endregion

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
                    //Debug.Log("Not won yet...");
                    return;
                }
            }
            Debug.Log("Game Won..");
        }

        void Start()
        {
            deck.Shuffle();
            StartGame();
        }

        public void StartGame()
        {
            int givenCards = 0;
            for (int i = 0; i < piles.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
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

        public IValidArea AutoMove(Card card)
        {
            foreach (var spot in spots)
            {
                if (spot.CanAppendCard(card.gameObject))
                {
                    Debug.Log(string.Format("[GameManager] card {0} can be moved onto {1}", card.name, spot.SpotName));
                    // TO DO: perform the move 
                    return spot;
                }
            }
            return null;
        }
        private void OnDestroy()
        {
            OnValidMove -= UpdateMovesCounter;
        }

    }
}
