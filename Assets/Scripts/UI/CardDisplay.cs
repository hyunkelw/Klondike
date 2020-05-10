using System.Collections;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.UI
{
    public class CardDisplay : MonoBehaviour
    {
        [SerializeField] private Image rankSR = default;
        [SerializeField] private Image suitSR = default;
        [SerializeField] private Image iconSR = default;

        [SerializeField] private Sprite[] rankSprites = default;
        [SerializeField] private Sprite[] suitSprites = default;

        private PlayableCard cardDetail = default;

        private void OnEnable()
        {
            //cardDetail = GetComponent<Card>().CardDetails;
            //ChangeCardDetails(cardDetail);
            GetComponent<Card>().OnValuesChanged += ChangeCardDetails;
        }

        public void ChangeCardDetails(PlayableCard newDetails)
        {
            if (newDetails != null)
            {
                cardDetail = newDetails;
                rankSR.sprite = ChooseRankSprite(newDetails.rank);
                rankSR.color = ChooseRankColor(newDetails.suit);
                suitSR.sprite = iconSR.sprite = ChooseSuitSprite(newDetails.suit);
            }
        }

        private void OnDestroy()
        {
            GetComponent<Card>().OnValuesChanged -= ChangeCardDetails;
        }

        private Color ChooseRankColor(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.HEARTS:
                case CardSuit.DIAMONDS:
                {
                    return Color.red;
                }
                case CardSuit.CLUBS:
                case CardSuit.SPADES:
                {
                    return Color.black;
                }
                default:
                {
                    return Color.white;
                }
            }
        }

        private Sprite ChooseSuitSprite(CardSuit suit)
        {
            return suitSprites[(int)suit - 1];
        }

        private Sprite ChooseRankSprite(CardRank rank)
        {
            return rankSprites[(int)rank - 1];
        }


        public IEnumerator Flip()
        {
            yield return null;
        }
    }

}