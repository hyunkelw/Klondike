﻿using System.Collections.Generic;
using Klondike.Core;
using UnityEngine;

namespace Klondike.Game
{
    public class Pile : MonoBehaviour, IValidArea
    {
        private readonly Vector2 ANCHOR_CENTER = new Vector2(.5f, .5f);

        #region Serialized Fields
        [SerializeField] private List<PlayableCard> onPileCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private GameObject cardPrefab = default;
        #endregion

        #region Attributes
        private LinkedList<GameObject> currentPile = new LinkedList<GameObject>();
        #endregion

        #region Properties
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
        public Vector3 SpotPosition
        {
            get
            {
                if (currentPile.Count > 0)
                {
                    return currentPile.Last.Value.GetComponent<Card>().appendSlot.GetComponent<RectTransform>().position;
                }
                else
                {
                    return GetComponent<RectTransform>().position;
                }
            }
        }
        #endregion

        private void OnEnable()
        {
            GameManager.OnStartGame += TurnNextCard;
            GameManager.OnValidMove += TurnNextCard;
        }

        /// <summary>
        /// Adds the given Card to the Pile
        /// </summary>
        /// <param name="cardToAdd"> the Card being Added to the Stock Pile</param>
        public void AddCardToPile(PlayableCard cardToAdd)
        {
            //Vector3 position = GetComponent<RectTransform>().anchoredPosition + new Vector2(0f, offset * currentPile.Count);
            var newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            newCard.GetComponent<Card>().SetCardDetails(cardToAdd);
            newCard.gameObject.name = cardToAdd.ToString();
            newCard.transform.SetParent(AppendSlot.GetComponent<RectTransform>(), false);
            RectTransform cardRT = newCard.GetComponent<RectTransform>();
            cardRT.anchorMin = cardRT.anchorMax = ANCHOR_CENTER;

            currentPile.AddLast(newCard);
            onPileCards.Add(cardToAdd); // only for showing in the inspector
        }

        /// <summary>
        /// Turn the card on top of the covered part of the pile, if there's one available and it's not flipped already.
        /// </summary>
        private void TurnNextCard()
        {
            if (currentPile.Count > 0)
            {
                var lastCard = currentPile.Last.Value.GetComponent<Card>();
                if (!lastCard.IsFaceUp)
                {
                    lastCard.Flip();
                }
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
            var children = cardGO.GetComponentsInChildren<Card>();
            foreach (var childCard in children)
            {
                currentPile.AddLast(childCard.gameObject);
                onPileCards.Add(childCard.CardDetails); // only for showing in the inspector
            }
        }

        private void OnDestroy()
        {
            GameManager.OnValidMove -= TurnNextCard;
            GameManager.OnStartGame -= TurnNextCard;
            currentPile.Clear();
        }

        /// <summary>
        /// Check if the given card can be appended to the to the Safe Spot.
        /// By the rules of the game, a card can be appended at the bottom of a Pile
        /// only if it has a different color and the immediately prior rank
        /// </summary>
        /// <param name="cardToAppend">the card to append</param>
        /// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
        public bool CanAppendCard(GameObject cardToAppendGO)
        {
            var cardToAppend = cardToAppendGO.GetComponent<Card>().CardDetails;
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

    }
}