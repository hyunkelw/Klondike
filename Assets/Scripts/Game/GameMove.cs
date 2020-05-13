using Klondike.Core;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Game
{
    public class GameMove
    {
        private IValidArea from, to;
        private GameObject card;
        private MoveType moveType;

        public int PointsAwarded { get; private set; } 
        public bool IsFlip { get { return moveType == MoveType.FLIP; } } 

        public GameMove(IValidArea leavingSpot, IValidArea landingSpot, GameObject whichCard)
        {
            from = leavingSpot;
            to = landingSpot;
            card = whichCard;

            int pointsToAward;
            switch (landingSpot)
            {
                // From Waste to Tableu
                case var dummyvar when leavingSpot is Deck && landingSpot is Pile:
                    pointsToAward = 5;
                    moveType = MoveType.WASTE_TO_TABLEAU;
                    break;
                // From Waste to a Foundation
                case var dummyvar when leavingSpot is Deck && landingSpot is Foundation:
                    pointsToAward = 10;
                    moveType = MoveType.WASTE_TO_FOUNDATION;
                    break;
                // From Tableau to Foundation
                case var dummyvar when leavingSpot is Pile && landingSpot is Foundation:
                    pointsToAward = 10;
                    moveType = MoveType.TABLEAU_TO_FOUNDATION;
                    break;
                // From Flipping a card
                case var dummyvar when leavingSpot == landingSpot && landingSpot is Pile:
                    pointsToAward = 5;
                    moveType = MoveType.FLIP;
                    break;
                // From Foundation to Tableau
                case var dummyvar when leavingSpot is Foundation && landingSpot is Pile:
                    pointsToAward = -15;
                    moveType = MoveType.FOUNDATION_TO_TABLEAU;
                    break;
                // From recycling the deck
                case var dummyvar when leavingSpot == landingSpot && landingSpot is Deck:
                    pointsToAward = -100;
                    moveType = MoveType.RECYCLE_WASTE;
                    break;
                default:
                    pointsToAward = 0;
                    moveType = MoveType.NONE;
                    break;
            }
            
            PointsAwarded = pointsToAward;
        }

        public override string ToString()
        {
            return string.Format("{0} - Moved from {1} to {2} - {3} points awarded", card,  from.SpotName, to.SpotName, PointsAwarded);
        }

        public void Execute()
        {
            if (moveType == MoveType.FLIP)
            {
                card.GetComponent<Card>().Flip();
            }
            else
            {
                card.GetComponent<Card>().startTravel(to.SpotPosition);
                from.DetachCard(card.gameObject);
                to.AppendCard(card.gameObject);
            }
        }


        public void Undo()
        {
            if (moveType == MoveType.FLIP)
            {
                card.GetComponent<Card>().Flip();
            }
            else
            {
                card.GetComponent<Card>().startTravel(from.SpotPosition);
                to.DetachCard(card.gameObject);
                from.AppendCard(card.gameObject);
            }
        }

    }
}
