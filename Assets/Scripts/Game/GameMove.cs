using Klondike.Core;
using Klondike.Utils;
using UnityEngine;

namespace Klondike.Game
{
    public class GameMove
    {
        #region Attributes
        private IValidArea from, to;
        #endregion

        #region Properties
        public int PointsAwarded { get; private set; }
        public MoveType MoveType { get; private set; }
        public GameObject Card { get; private set; }
        public bool IsFlip { get { return MoveType == MoveType.FLIP; } }
        #endregion

        #region Constructors and Overrides
        public GameMove(IValidArea leavingSpot, IValidArea landingSpot, GameObject whichCard)
        {
            from = leavingSpot;
            to = landingSpot;
            Card = whichCard;

            int pointsToAward;
            switch (landingSpot)
            {
                // From Waste to Tableu
                case var dummyvar when leavingSpot is Deck && landingSpot is Pile:
                    pointsToAward = 5;
                    MoveType = MoveType.WASTE_TO_TABLEAU;
                    break;
                // From Waste to a Foundation
                case var dummyvar when leavingSpot is Deck && landingSpot is Foundation:
                    pointsToAward = 10;
                    MoveType = MoveType.WASTE_TO_FOUNDATION;
                    break;
                // From Tableau to Foundation
                case var dummyvar when leavingSpot is Pile && landingSpot is Foundation:
                    pointsToAward = 10;
                    MoveType = MoveType.TABLEAU_TO_FOUNDATION;
                    break;
                // From Flipping a card
                case var dummyvar when leavingSpot == landingSpot && landingSpot is Pile:
                    pointsToAward = 5;
                    MoveType = MoveType.FLIP;
                    break;
                // From Foundation to Tableau
                case var dummyvar when leavingSpot is Foundation && landingSpot is Pile:
                    pointsToAward = -15;
                    MoveType = MoveType.FOUNDATION_TO_TABLEAU;
                    break;
                // From fetching a card
                case var dummyvar when leavingSpot == landingSpot && landingSpot is Deck && whichCard != null:
                    pointsToAward = 0;
                    MoveType = MoveType.FETCH_CARD;
                    break;
                // From recycling the deck
                case var dummyvar when leavingSpot == landingSpot && landingSpot is Deck && whichCard == null:
                    pointsToAward = 0;
                    MoveType = MoveType.RECYCLE_WASTE;
                    break;
                default:
                    pointsToAward = 0;
                    MoveType = MoveType.NONE;
                    break;
            }

            PointsAwarded = pointsToAward;
        }

        public override string ToString()
        {
            return string.Format("{0} - Moved from {1} to {2} - {3} points awarded", Card, from.SpotName, to.SpotName, PointsAwarded);
        }
        #endregion

        public void Execute()
        {
            switch (MoveType)
            {
                case MoveType.FLIP:
                {
                    Card.GetComponent<Card>().StartFlip();
                    break;
                }
                case MoveType.FETCH_CARD:
                case MoveType.RECYCLE_WASTE:
                //case MoveType.WASTE_TO_TABLEAU:
                //case MoveType.WASTE_TO_FOUNDATION:
                {
                    from.Execute?.Invoke(this);
                    break;
                }
                default:
                {
                    Card.GetComponent<Card>().startTravel(to.SpotPosition);
                    from.DetachCard(Card.gameObject);
                    to.AppendCard(Card.gameObject);
                    break;
                }
            }
        }


        public void Undo()
        {
            switch (MoveType)
            {
                case MoveType.FLIP:
                {
                    Card.GetComponent<Card>().StartFlip();
                    break;
                }
                case MoveType.FETCH_CARD:
                case MoveType.RECYCLE_WASTE:
                //case MoveType.WASTE_TO_TABLEAU:
                //case MoveType.WASTE_TO_FOUNDATION:
                {
                    from.Undo?.Invoke(this);
                    break;
                }
                default:
                {
                    Card.GetComponent<Card>().startTravel(from.SpotPosition);
                    to.DetachCard(Card.gameObject);
                    from.AppendCard(Card.gameObject);
                    break;
                }

            }
        }

    }
}
