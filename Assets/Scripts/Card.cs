using UnityEngine;
using Klondike.Utils;

namespace Klondike.Core
{

    [System.Serializable]
    public class Card
    {
        public CardSuit suit = CardSuit.NONE;
        public CardRank rank = CardRank.NONE;
        public CardColor color = CardColor.NONE;

        public Card(int suitIndex, int rankIndex)
        {
            suit = (CardSuit)suitIndex;
            rank = (CardRank)rankIndex;
            switch (suit)
            {
                case CardSuit.DIAMONDS:
                case CardSuit.HEARTS:
                {
                    color = CardColor.RED;
                    break;
                }
                case CardSuit.CLUBS:
                case CardSuit.SPADES:
                {
                    color = CardColor.BLACK;
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} of {2} - {3}", (int)rank + ((int)suit - 1) * 13, rank, suit, color);
        }
    }
}