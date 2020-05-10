using Klondike.Utils;

namespace Klondike.Core
{

    [System.Serializable]
    public class PlayableCard
    {
        public CardSuit suit = CardSuit.NONE;
        public CardRank rank = CardRank.NONE;
        public CardColor cardColor = CardColor.NONE;

        public PlayableCard(int suitIndex, int rankIndex)
        {
            suit = (CardSuit)suitIndex;
            rank = (CardRank)rankIndex;
            switch (suit)
            {
                case CardSuit.DIAMONDS:
                case CardSuit.HEARTS:
                {
                    cardColor = CardColor.RED;
                    break;
                }
                case CardSuit.CLUBS:
                case CardSuit.SPADES:
                {
                    cardColor = CardColor.BLACK;
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
            return string.Format("{0} - {1} of {2} - {3}", (int)rank + ((int)suit - 1) * 13, rank, suit, cardColor);
        }

    }
}