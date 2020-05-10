using System.Collections;
using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.UI
{
    public class UI_CardDisplay : MonoBehaviour
    {
        [SerializeField] private Image rankSR = default;
        [SerializeField] private Image suitSR = default;
        [SerializeField] private Image iconSR = default;
        [SerializeField] private Image coverSR = default;
        [SerializeField] private float rotationTime = .2f;

        [SerializeField] private Sprite[] rankSprites = default;
        [SerializeField] private Sprite[] suitSprites = default;

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


        public IEnumerator Flip(bool toFaceUp)
        {
            // First, rotate to 90 degrees 
            var startingRotation = Quaternion.identity;
            var endingRotation = Quaternion.AngleAxis(90f, toFaceUp ? Vector3.up : Vector3.down);
            for (var t = 0f; t < 1; t += Time.deltaTime / rotationTime)
            {
                transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, t);
                yield return null;
            }
            // then, deactivate the cover sprite 
            coverSR.gameObject.SetActive(!toFaceUp);

            // finally, return to initial position
            startingRotation = transform.rotation;
            endingRotation = Quaternion.identity;
            var progress = 0f;
            while (transform.rotation != endingRotation)
            {
                progress += Time.deltaTime / rotationTime;
                transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, progress);
                yield return null;
            }
            //for (var t = 0f; t < 1; t += Time.deltaTime / rotationTime)
            //{
            //    transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, t);
            //    yield return null;
            //}

        }
    }

}