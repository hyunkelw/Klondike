using System;
using Klondike.Core;
using TMPro;
using UnityEngine;

namespace Klondike.Game
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gameTimer = default;

        private bool gameStarted;
        private float preparationTime = 0f, elapsedTime = 0f;

        public int ElapsedTime { get { return TimeSpan.FromSeconds(elapsedTime - preparationTime).Seconds; } }

        private void OnEnable()
        {
            GameManager.OnStartGame += GameStarted;
        }

        void Update()
        {
            if (!gameStarted)
            {
                preparationTime += 1 * Time.deltaTime;
            }
            else
            {
                UpdateLevelTimer();
            }
        }

        private void UpdateLevelTimer()
        {
            elapsedTime += 1 * Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(elapsedTime - preparationTime);
            gameTimer.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        }

        private void GameStarted()
        {
            gameStarted = true;
        }

        private void OnDisable()
        {
            GameManager.OnStartGame -= GameStarted;
        }

    }
}
