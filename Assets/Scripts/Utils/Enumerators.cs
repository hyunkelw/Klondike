namespace Klondike.Utils
{
    public enum CardSuit { NONE, HEARTS, DIAMONDS, CLUBS, SPADES, COUNT }
    
	public enum CardColor { NONE, BLACK, RED }

    public enum CardRank { NONE, ACE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, J, Q, K, COUNT }

    public enum Constraint { PORTRAIT, LANDSCAPE }

	public enum AnchorType
	{
		BOTTOMLEFT,
		BOTTOMCENTER,
		BOTTOMRIGHT,
		MIDDLELEFT,
		MIDDLECENTER,
		MIDDLERIGHT,
		TOPLEFT,
		TOPCENTER,
		TOPRIGHT,
	}
}