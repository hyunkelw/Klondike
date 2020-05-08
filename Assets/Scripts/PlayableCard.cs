using UnityEngine;
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

        /// <summary>
        /// Check if the given card can be appended to the parent Card.
        /// By the rules of the game, a card can be appended only if it has a different color and the immediately prior rank
        /// </summary>
        /// <param name="parentCard">the potential child</param>
        /// <param name="cardToAppend"> the potential parent</param>
        /// <returns></returns>
        public static bool CanBeAppended(PlayableCard cardToAppend, PlayableCard parentCard)
        {
            Debug.Log(string.Format("Attempting to append {0} to {1}", cardToAppend, parentCard));
            bool canBeAppended = (parentCard.cardColor != cardToAppend.cardColor && (int)cardToAppend.rank == (int)parentCard.rank - 1);
            return canBeAppended;
        }
    }
}