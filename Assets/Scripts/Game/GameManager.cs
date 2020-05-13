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
        public static Action OnStartGame, OnFoundationsUpdated, OnEndGame;
        public static Action OnValidMove;

        #region Serialized Fields
        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck gameDeck = default;
        [SerializeField] private Foundation[] foundations = default;
        [SerializeField] private RectTransform spawnPoint = default;
        [SerializeField] private GameObject cardPrefab = default;
        #endregion

        #region Attributes
        private PlayableDeck deck = new PlayableDeck();
        private List<IValidArea> spots = new List<IValidArea>();
        private Stack<GameMove> movesList = new Stack<GameMove>();
        #endregion

        #region Properties
        public int Score { get; private set; } = 0;
        public int Moves { get; private set; } = 0;
        public int RevertableMoves { get { return movesList.Count; } }
        #endregion

        private void Awake()
        {
            SetupSingleton();
            spots.AddRange(foundations);
            spots.AddRange(piles);
        }


        private void OnEnable()
        {
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
                gameDeck.AddCardToStock(deck.GetNextCard());
                yield return new WaitForEndOfFrame();
                givenCards++;
            }
            OnStartGame?.Invoke();
        }

        public void HandleNewMove(GameMove move)
        {
            movesList.Push(move);
            Score += move.PointsAwarded;
            if (!move.IsFlip)
            {
                Moves++;
            }
            move.Execute();
            OnValidMove?.Invoke();
        }

        public void UndoMove()
        {
            if(movesList.Count == 0) { return; }

            var move = movesList.Pop();
            Score -= move.PointsAwarded;
            move.Undo();
            if (move.IsFlip)
            {
                move = movesList.Pop();
                move.Undo();
                Score -= move.PointsAwarded;
            }
            Moves++;
            OnValidMove?.Invoke();
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
            pile.AddCardToPile(newCard, lastCardOfPile);

        }

        public IValidArea Hint(Card card)
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

    }
}
