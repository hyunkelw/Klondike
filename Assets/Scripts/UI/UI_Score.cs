using UnityEngine;
using TMPro;
using Klondike.Game;

namespace Klondike.UI
{
    public class UI_Score : MonoBehaviour
    {
        /*Reference to Score counter text */
        [SerializeField] private TextMeshProUGUI scoreCounter = default;

        public void UpdateScoreCounter()
        {
            if (!scoreCounter)
            {
                Debug.LogWarning("[UI_Score] Score Counter not setted!");
                return;
            }
            scoreCounter.text = GameManager.Singleton.Score.ToString();
        }

    }
}