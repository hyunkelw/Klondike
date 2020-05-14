using System;
using Klondike.Core;
using TMPro;
using UnityEngine;

namespace Klondike.Game
{
    public class GameTimer : MonoBehaviour
    {

        #region Serialized Fields
        [SerializeField] private TextMeshProUGUI gameTimer = default;
        #endregion

        #region Attributes
        private bool gameStarted = false;
        private float preparationTime = 0f, elapsedTime = 0f;
        #endregion

        #region Properties
        public int ElapsedTime { get { return TimeSpan.FromSeconds(elapsedTime - preparationTime).Seconds; } } 
        #endregion

        private void OnEnable()
        {
            GameManager.OnStartGame += ToggleGameStatus;
            GameManager.OnEndGame += ToggleGameStatus;
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

            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
            //if (time.Seconds % 10 == 0 )
            //{
            //    GameManager.Singleton.AddPointsToScore(-2);
            //}
            gameTimer.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        }

        private void ToggleGameStatus()
        {
            gameStarted = !gameStarted;
        }

        private void OnDisable()
        {
            GameManager.OnStartGame -= ToggleGameStatus;
            GameManager.OnEndGame -= ToggleGameStatus;
        }

    }
}
