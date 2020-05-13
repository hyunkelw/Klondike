using Klondike.Game;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.UI
{
    [ExecuteInEditMode]
    public class UI_Foundation : MonoBehaviour
    {
        [SerializeField] private Image suitSR = default;
        [SerializeField] private Sprite[] suitSprites = default;

        private void OnEnable()
        {
            GetComponentInParent<Foundation>().OnValidated += ChangeSuitDetails;
                //ChangeSuitDetails(Suit);
        }

        public void ChangeSuitDetails(CardSuit newSuit)
        {
            if (newSuit != CardSuit.NONE)
            {
                 suitSR.sprite = ChooseSuitSprite(newSuit);
            }
        }

        private Sprite ChooseSuitSprite(CardSuit suit)
        {
            return suitSprites[(int)suit - 1];
        }

        private void OnDisable()
        {
            var foundation = GetComponentInParent<Foundation>();
            if (foundation != null)
            {
                foundation.OnValidated -= ChangeSuitDetails;
            }
        }

    }

}