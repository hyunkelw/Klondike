using Klondike.Core;
using Klondike.Utils;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private Card cardDetail = default;
    
    [SerializeField] private Image rankSR = default;
    [SerializeField] private Image suitSR = default;
    [SerializeField] private Image iconSR = default;

    [SerializeField] private Sprite[] rankSprites = default;
    [SerializeField] private Sprite[] suitSprites = default;
    
    // Start is called before the first frame update
    void Start()
    {
        ChangeCardDetails();
    }


    private void OnValidate()
    {
        ChangeCardDetails();
    }

    private void ChangeCardDetails()
    {
        if (cardDetail != null)
        {
            rankSR.sprite = ChooseRankSprite(cardDetail.rank);
            rankSR.color = ChooseRankColor(cardDetail.color);
            suitSR.sprite = ChooseSuitSprite(cardDetail.suit);
            iconSR.sprite = ChooseSuitSprite(cardDetail.suit);
        }
    }

    private Color ChooseRankColor(CardColor color)
    {
        switch (color)
        {
            case CardColor.BLACK:
                return Color.black;
            case CardColor.RED:
                return Color.red;
            default:
                return Color.white;
        }
    }

    private Sprite ChooseSuitSprite(CardSuit suit)
    {
        return suitSprites[(int)suit - 1];
    }

    private Sprite ChooseRankSprite(CardRank rank)
    {
        return rankSprites[(int)rank - 1];
    }

}
