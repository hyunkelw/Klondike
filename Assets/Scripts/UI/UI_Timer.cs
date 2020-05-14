using UnityEngine;
using TMPro;
using Klondike.Game;
using System;

namespace Klondike.UI
{
    public class UI_Timer : MonoBehaviour
    {
        /*Reference to Timer counter text */
        [SerializeField] private TextMeshProUGUI timeDisplay = default;

        public void UpdateTimerText()
        {
            if (!timeDisplay)
            {
                Debug.LogWarning("[UI_Timer] Timer Text not setted!");
                return;
            }
            var time = TimeSpan.FromSeconds(FindObjectOfType<GameTimer>().ElapsedTime);
            timeDisplay.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
        }

    }
}