using System;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Core
{
    public class CardInfo : MonoBehaviour
    {
        [SerializeField] private PlayableCard cardDetail = default;

        public PlayableCard Card { get { return cardDetail; } }

        public Action<PlayableCard> OnValuesChanged;

        public void SetCardDetails(PlayableCard card)
        {
            cardDetail = card;
            OnValuesChanged?.Invoke(cardDetail);
        }

        private void OnValidate()
        {
            OnValuesChanged?.Invoke(cardDetail);
        }


    }

}