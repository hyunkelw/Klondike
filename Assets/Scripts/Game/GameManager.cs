using System;
using System.Collections;
using System.Collections.Generic;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Game
{

    public class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnValidMove, OnStartGame, OnFoundationsUpdated, OnEndGame;

        #region Serialized Fields
        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck stock = default;
        [SerializeField] private Foundation[] foundations = default;
        [SerializeField] private RectTransform spawnPoint = default;
        [SerializeField] private GameObject cardPrefab = default;
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
                    return;
                }
            }
            OnEndGame?.Invoke();
            Debug.Log("[GameManager] Game Won..");
        }

        void Start()
        {
            deck.Shuffle();
            StartCoroutine(StartGame());
        }

        public IEnumerator StartGame()
        {
            int givenCards = 0;
            for (int i = 0; i < piles.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    yield return StartCoroutine(CreateCard(piles[i], deck.GetNextCard(), j == i));
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

        public IEnumerator CreateCard(Pile pile, PlayableCard cardToAdd, bool lastCardOfPile)
        {

            var newCard = Instantiate(cardPrefab);
            yield return new WaitForSeconds(.15f);

            newCard.GetComponent<Card>().SetCardDetails(cardToAdd);
            newCard.gameObject.name = cardToAdd.ToString();
            newCard.transform.SetParent(pile.AppendSlot.GetComponent<RectTransform>(), false);
            newCard.GetComponent<RectTransform>().position = spawnPoint.position;

            StartCoroutine(newCard.GetComponent<Card>().TravelTo(pile.SpotPosition, lastCardOfPile));
            pile.AddCardToPile(newCard);

        }

        public IValidArea AutoMove(Card card)
        {
            foreach (var spot in spots)
            {
                if (spot.CanAppendCard(card.gameObject))
                {
                    Debug.Log(string.Format("[GameManager] card {0} can be moved onto {1}", card.name, spot.SpotName));
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
