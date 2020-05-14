using System;
using Klondike.Utils;

namespace Klondike.Core
{
    public class PlayableDeck
    {
        public const int DECK_SIZE = 52;

        private PlayableCard[] deckArray = new PlayableCard[DECK_SIZE];
        private int currentIndex = 0;

        public PlayableDeck()
        {
            for (int deckIndex = 0, suitIndex = 1; deckIndex < deckArray.Length; suitIndex++)
            {
                for (int rankIndex = 1; rankIndex < (int)CardRank.COUNT; deckIndex++, rankIndex++)
                {
                    deckArray[deckIndex] = new PlayableCard(suitIndex, rankIndex);
                }
            }
        }

        public void Shuffle()
        {
            var seed = new Random(); 
            for (int i = 0; i < deckArray.Length - 1; i++)
            {
                int cardToSwapIndex = i + seed.Next(deckArray.Length - i);
                var cardToSwap = deckArray[cardToSwapIndex];
                deckArray[cardToSwapIndex] = deckArray[i];
                deckArray[i] = cardToSwap;
            }
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        public PlayableCard GetNextCard()
        {
            return deckArray[currentIndex++];
        }


        public override string ToString()
        {
            string toRet = "";
            for (int i = 0; i < deckArray.Length; i++)
            {
                toRet = toRet + deckArray[i].ToString() + "\n";
            }
            return toRet;
        }

    }
}