using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klondike.Core
{
    //public class Pile : MonoBehaviour, IDropHandler
    public class Pile : MonoBehaviour
    {
        private readonly Vector2 ANCHOR_CENTER = new Vector2(.5f, .5f);

        [SerializeField] private List<PlayableCard> availableCards = new List<PlayableCard>(); // only for showing in the inspector
        [SerializeField] private List<PlayableCard> onStackCards = new List<PlayableCard>(); // only for showing in the inspector

        [SerializeField] private GameObject cardPrefab = default;
        [SerializeField] private GameObject coveredCardPrefab = default;
        [SerializeField] private float offset = -15f;

        private Stack<PlayableCard> coveredPile = new Stack<PlayableCard>();
        private GameObject bottomCard = default; 

        public PlayableCard AttachableCard { get { return bottomCard.GetComponent<Card>().CardDetails;  } }
        public GameObject AttachableCardSlot { get { return bottomCard.GetComponent<Card>().appendSlot;  } }

        public int CoveredPileStartingSize { get; set; }

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
            //var newCoveredCard = Instantiate(coveredCardPrefab, transform.position, Quaternion.identity);
            newCoveredCard.gameObject.name = "Covered Card" + coveredPile.Count;
            newCoveredCard.transform.SetParent(transform, false);
            //newCoveredCard.GetComponent<Card>().SetCardDetails(cardToAdd);

            availableCards.Add(cardToAdd); // only for showing in the inspector
        }


        /// <summary>
        /// Turn the card on top of the covered part of the pile, if there's one available and no flipped card.
        /// </summary>
        // TODO: this must be done automatically ONLY after a valid move.
        private void TurnNextAvailableCard()
        {
            if (bottomCard == null && coveredPile.Count > 0)
            {
                PlayableCard turnedCard = coveredPile.Pop();
                bottomCard = Instantiate(cardPrefab, transform.position, Quaternion.identity);
                bottomCard.gameObject.name = turnedCard.ToString();
                bottomCard.transform.SetParent(transform, false);
                bottomCard.GetComponent<Card>().SetCardDetails(turnedCard);
                Debug.Log(string.Format("Card {0} moved from covered to uncovered", turnedCard));
                if (coveredPile.Count > 0)
                {
                    Debug.Log("Next available card in pile: " + coveredPile.Peek());
                }

                availableCards.Remove(turnedCard); // only for showing in the inspector
                onStackCards.Add(turnedCard); // only for showing in the inspector
            }
        }

        public void AppendCard(GameObject cardGO)
        {
            RectTransform cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.SetParent(AttachableCardSlot.GetComponent<RectTransform>(), false);
            cardRT.anchorMin = new Vector2(.5f, .5f);
            cardRT.anchorMax = new Vector2(.5f, .5f);
            bottomCard = cardGO;
        }

        private void OnDestroy()
        {
            //GameManager.OnValidMove -= TurnNextAvailableCard;
            coveredPile.Clear();
            bottomCard = null;

        }

        //public void OnDrop(PointerEventData eventData)
        //{
        //    if (eventData.pointerDrag == null) { return; }


        //    Debug.Log("Dropped Card on me");

        //    if (PlayableCard.CanBeAppended(lastAvailableCard.GetComponent<CardInfo>().Card, eventData.pointerDrag.GetComponent<CardInfo>().Card))
        //    {
        //        //        lastAvailableCard.GetComponent<Card>().AppendCard(eventData.pointerDrag.GetComponent<Card>());
        //        //    }
        //        //    else
        //        //    {
        //        //    }
        //    }
        //}


    }
}