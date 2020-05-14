using System;
using Klondike.Game;
using Klondike.UI;
using TMPro;
using UnityEngine;

public class UI_WinPanel : MonoBehaviour
{
    /*Reference to Score counter text */
    [SerializeField] private TextMeshProUGUI scoreCounter = default;
    /*Reference to moves counter text */
    [SerializeField] private TextMeshProUGUI movesCounter = default;
    /*Reference to Timer counter text */
    [SerializeField] private TextMeshProUGUI timeDisplay = default;

    private void OnEnable()
    {
        UpdateWinValues();
    }

    public void UpdateWinValues()
    {
        movesCounter.text = GameManager.Singleton.Moves.ToString();
        scoreCounter.text = GameManager.Singleton.Score.ToString();
        var time = TimeSpan.FromSeconds(FindObjectOfType<GameTimer>().ElapsedTime);
        timeDisplay.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }

}
