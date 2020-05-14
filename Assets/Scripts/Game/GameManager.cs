using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Klondike.Game
{

    public class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnStartGame, OnFoundationsUpdated, OnEndGame;
        public static Action OnValidMove, OnScoreUpdated;

        public const int BONUS_POINTS_BASE = 20000;
        public const int BONUS_POINTS_FACTOR = 35;
        public const int BONUS_POINTS_TIMER_THRESHOLD = 30;

        #region Serialized Fields
        [SerializeField] private Pile[] piles = default;
        [SerializeField] private Deck gameDeck = default;
        [SerializeField] private Foundation[] foundations = default;
        [SerializeField] private RectTransform spawnPoint = default;
        [SerializeField] private GameObject cardPrefab = default;
        [SerializeField] private int score = 0;
        #endregion

        #region Attributes
        private PlayableDeck deck = new PlayableDeck();
        private List<IValidArea> spots = new List<IValidArea>();
        private Stack<GameMove> movesList = new Stack<GameMove>();
        #endregion

        #region Properties
        public int Score { get { return score; } }
        public int Moves { get; private set; } = 0;
        public int RevertableMoves { get { return movesList.Count; } }
        #endregion

        private void Awake()
        {
            SetupSingleton();
        }

        public void AddPointsToScore(int points)
        {
            score = score + points >= 0 ? score + points : 0;
            OnScoreUpdated?.Invoke();
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

            var totalTime = FindObjectOfType<GameTimer>().ElapsedTime;
            if (totalTime > BONUS_POINTS_TIMER_THRESHOLD)
            {
                score += (BONUS_POINTS_BASE / totalTime) * BONUS_POINTS_FACTOR;
            }
            OnEndGame?.Invoke();

            Debug.Log("[GameManager] Game Won..");
        }

        void Start()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            ResetGame();
            deck = new PlayableDeck();
            deck.Shuffle();
            Debug.Log("Starting new game");
            StartCoroutine(InitializeAndStart());
        }

        public void ReplayGame()
        {
            ResetGame();
            deck.Reset();
            Debug.Log("Starting new game");
            StartCoroutine(InitializeAndStart());
        }

        private IEnumerator InitializeAndStart()
        {
            var sceneLoading = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            while (!sceneLoading.isDone)
            {
                yield return null;
            }
            foundations = FindObjectsOfType<Foundation>();
            spots.AddRange(foundations);
            piles = FindObjectsOfType<Pile>();
            piles = piles.OrderBy(x => x.name).ToArray<Pile>();
            spots.AddRange(piles);
            gameDeck = FindObjectOfType<Deck>();
            spawnPoint = gameDeck.SpawnPoint;
            StartCoroutine(StartGame());
        }

        public IEnumerator StartGame()
        {
            int givenCards = 0;
            for (int i = 0; i < piles.Length; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    yield return StartCoroutine(CreateCardForPile(piles[i], deck.GetNextCard(), j == i));
                    givenCards++;
                }

            }
            OnStartGame?.Invoke();
            while (givenCards < PlayableDeck.DECK_SIZE)
            {
                CreateCardForDeck(deck.GetNextCard());
                givenCards++;
            }
        }

         public void HandleNewMove(GameMove move)
        {
            movesList.Push(move);
            AddPointsToScore(move.PointsAwarded);
            if (move.MoveType == MoveType.RECYCLE_WASTE)
            {
                AddPointsToScore(-100);
            }
            if (move.MoveType != MoveType.FLIP)
            {
                Moves++; // a flip is always associated to another move. So, increasing the counter will be done by the associated move;
            }
            move.Execute();
            OnValidMove?.Invoke();
        }

        public void UndoMove()
        {
            if (movesList.Count == 0) { return; }

            var move = movesList.Pop();
            AddPointsToScore(-move.PointsAwarded);
            move.Undo();
            if (move.MoveType != MoveType.FLIP)
            {
                Moves++;
                OnValidMove?.Invoke();
            }
            else
            {
                UndoMove(); // a flip is always associated to another move and doesn't count like one. So, undo that as well;
            }
        }

        public IEnumerator CreateCardForPile(Pile pile, PlayableCard cardToAdd, bool lastCardOfPile)
        {

            var newCard = Instantiate(cardPrefab);
            yield return new WaitForSeconds(.1f);

            newCard.GetComponent<Card>().SetCardDetails(cardToAdd);
            newCard.gameObject.name = cardToAdd.ToString();
            newCard.transform.SetParent(pile.AppendSlot.GetComponent<RectTransform>(), false);
            newCard.GetComponent<RectTransform>().position = spawnPoint.position;

            Coroutine travelCoroutine = StartCoroutine(newCard.GetComponent<Card>().TravelTo(pile.SpotPosition));
            if (lastCardOfPile)
            {
                //yield return travelCoroutine;
                StartCoroutine(newCard.GetComponent<Card>().Flip());
            }
            pile.AddCardToPile(newCard, lastCardOfPile);
        }

        private void CreateCardForDeck(PlayableCard playableCard)
        {
            var newCard = Instantiate(cardPrefab);
            newCard.GetComponent<Card>().SetCardDetails(playableCard);
            newCard.gameObject.name = playableCard.ToString();

            newCard.transform.SetParent(gameDeck.GetComponent<RectTransform>(), false); // non mi serve sia figlia delle rect transform degli slot 
            newCard.transform.SetAsFirstSibling();
            
            newCard.GetComponent<RectTransform>().position = spawnPoint.position;
            gameDeck.AddCardToStock(newCard);
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

        public void ResetGame()
        {
            Moves = 0;
            score = 0;
            movesList.Clear();
            spots.Clear();
            Time.timeScale = 1f;
        }
    }
}
