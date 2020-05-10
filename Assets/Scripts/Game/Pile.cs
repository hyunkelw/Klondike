﻿using System.Collections.Generic;
using UnityEngine;

namespace Klondike.Core
{
    public class Pile : MonoBehaviour, IValidArea
    {
        private readonly Vector2 ANCHOR_CENTER = new Vector2(.5f, .5f);

        [SerializeField] private List<PlayableCard> availableCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private List<PlayableCard> onPileCards = new List<PlayableCard>(); // only for showing in the inspector

        [SerializeField] private GameObject cardPrefab = default;
        [SerializeField] private GameObject coveredCardPrefab = default;
        [SerializeField] private float offset = -15f;

        private Stack<PlayableCard> coveredPile = new Stack<PlayableCard>();
        private LinkedList<GameObject> currentPile = new LinkedList<GameObject>();

        private GameObject topCard = default;
        private GameObject bottomCard = default;

        public PlayableCard TopCard { get { return currentPile.First.Value.GetComponent<Card>().CardDetails; } }
        public PlayableCard BottomCard { get { return currentPile.Last.Value.GetComponent<Card>().CardDetails; } }
        public GameObject AppendSlot
        {
            get
            {
                if (currentPile.Count > 0)
                {
                    return currentPile.Last.Value.GetComponent<Card>().appendSlot;
                }
                else
                {
                    return gameObject;
                }

            }
        }

        public string SpotName { get { return gameObject.name; } }

        //public PlayableCard BottomCard { get { return  bottomCard.GetComponent<Card>().CardDetails;  } }
        //public GameObject AppendSlot { get { return bottomCard.GetComponent<Card>().appendSlot;  } }
        //public int CoveredPileStartingSize { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            // FOR TESTING PURPOSES ONLY
            foreach (var card in availableCards)
            {
                coveredPile.Push(card);
            }
            //TurnNextAvailableCard();
        }

        private void OnEnable()
        {
            GameManager.OnValidMove += TurnNextAvailableCard;
        }

        /// <summary>
        /// Adds the given Card to the covered part of the Pile
        /// </summary>
        /// <param name="cardToAdd"> the Card being Added to the Stock Pile</param>
        public void AddCoveredCardToPile(PlayableCard cardToAdd)
        {
            coveredPile.Push(cardToAdd);
            Vector3 position = GetComponent<RectTransform>().anchoredPosition + new Vector2(0f, offset * coveredPile.Count);
            var newCoveredCard = Instantiate(coveredCardPrefab, position, Quaternion.identity);
            newCoveredCard.gameObject.name = "Covered Card" + coveredPile.Count;
            newCoveredCard.transform.SetParent(transform, false);

            availableCards.Add(cardToAdd); // only for showing in the inspector
        }


        /// <summary>
        /// Turn the card on top of the covered part of the pile, if there's one available and no flipped card.
        /// </summary>
        // TODO: this must be done automatically ONLY after a valid move.
        private void TurnNextAvailableCard()
        {
            if (currentPile.Count == 0 && coveredPile.Count > 0)
            {
                PlayableCard turnedCard = coveredPile.Pop();

                var newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity);

                newCard.gameObject.name = turnedCard.ToString();
                newCard.transform.SetParent(transform, false);
                newCard.GetComponent<Card>().SetCardDetails(turnedCard);


                Debug.Log(string.Format("Card {0} turned on {1}", turnedCard, SpotName));

                currentPile.AddLast(newCard);
                availableCards.Remove(turnedCard); // only for showing in the inspector
                onPileCards.Add(turnedCard); // only for showing in the inspector
            }
        }

        /// <summary>
        /// Detach the card (and every card attached below) from this pile
        /// </summary>
        /// <param name="cardGO"></param>
        public void DetachCard(GameObject cardGO)
        {
            Debug.Log(string.Format("[Pile] Attempting to remove card {0} from {1}", cardGO.gameObject.name, SpotName));

            // Before Detaching the card, check if it's not the bottom card. If it is, need to detach everything from that point onwards
            try
            {
                var node = currentPile.Find(cardGO);
                while (node != null)
                {
                    var nextNode = node.Next;
                    currentPile.Remove(node);
                    onPileCards.Remove(node.Value.GetComponent<Card>().CardDetails); // only for showing in the inspector
                    node = nextNode;
                }
                GameManager.OnValidMove?.Invoke();
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError(string.Format("[Pile] Couldn't remove card {0} from {1}", cardGO.gameObject.name, SpotName));
                Debug.LogError(e.Message);
            }

        }

        public void AppendCard(GameObject cardGO)
        {
            RectTransform cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.SetParent(AppendSlot.GetComponent<RectTransform>(), false);
            cardRT.anchorMin = cardRT.anchorMax = ANCHOR_CENTER;
            // Add the card and all his children to the current Pile
            //currentPile.AddLast(cardGO);
            //onPileCards.Add(cardGO.GetComponent<Card>().CardDetails); // only for showing in the inspector

            var children = cardGO.GetComponentsInChildren<Card>();
            foreach (var childCard in children)
            {
                currentPile.AddLast(childCard.gameObject);
                onPileCards.Add(childCard.CardDetails); // only for showing in the inspector
            }
            GameManager.OnValidMove?.Invoke();
        }


        private void OnDestroy()
        {
            //GameManager.OnValidMove -= TurnNextAvailableCard;
            coveredPile.Clear();
            currentPile.Clear();
            //bottomCard = null;
        }

        /// <summary>
        /// Check if the given card can be appended to the to the Safe Spot.
        /// By the rules of the game, a card can be appended at the bottom of a Pile
        /// only if it has a different color and the immediately prior rank
        /// </summary>
        /// <param name="cardToAppend">the card to append</param>
        /// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
        public bool CanAppendCard(PlayableCard cardToAppend)
        {
            bool canBeAppended;
            // if the pile is empty, the only appendable card is a King
            if (currentPile.Count == 0)
            {
                canBeAppended = (cardToAppend.rank == Utils.CardRank.K);
            }
            else
            {
                var parentCard = currentPile.Last.Value.GetComponent<Card>().CardDetails;
                canBeAppended = (parentCard.cardColor != cardToAppend.cardColor && (int)cardToAppend.rank == (int)parentCard.rank - 1);
                Debug.Log(string.Format("Attempting to append {0} to {1} - {2}", cardToAppend, parentCard, canBeAppended ? "Success" : "Failed"));
            }
            return canBeAppended;
        }

        /// <summary>
        /// Turn the card on top of the covered part of the pile, if there's one available and no flipped card.
        /// </summary>
        // TODO: this must be done automatically ONLY after a valid move.
        private void TurnNextAvailableCard_OLD()
        {
            if (topCard == null && coveredPile.Count > 0)
            {
                PlayableCard turnedCard = coveredPile.Pop();
                //topCard = Instantiate(cardPrefab, transform.position, Quaternion.identity);
                //topCard.gameObject.name = turnedCard.ToString();
                //topCard.transform.SetParent(transform, false);
                //topCard.GetComponent<Card>().SetCardDetails(turnedCard);
                //bottomCard = topCard;
                //Debug.Log(string.Format("Card {0} moved from covered to uncovered", turnedCard));
                if (coveredPile.Count > 0)
                {
                    //Debug.Log("Next available card in pile: " + coveredPile.Peek());
                }

                availableCards.Remove(turnedCard); // only for showing in the inspector
                onPileCards.Add(turnedCard); // only for showing in the inspector
            }
        }

        public void AppendCard_OLD(GameObject cardGO)
        {
            RectTransform cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.SetParent(AppendSlot.GetComponent<RectTransform>(), false);
            cardRT.anchorMin = ANCHOR_CENTER;
            cardRT.anchorMax = ANCHOR_CENTER;
            bottomCard = cardGO;
        }

    }
}