using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.UI
{

    public class UI_Foundation : MonoBehaviour
    {
        [SerializeField] private Image suitSR = default;
        [SerializeField] private Sprite[] suitSprites = default;

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
    }

}