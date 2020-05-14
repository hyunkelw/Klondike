using System;
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
        private bool timerStarted = false;
        private float elapsedTime = 0f;
        #endregion

        #region Properties
        public int ElapsedTime { get { return (int)elapsedTime; } } 
        #endregion

        private void OnEnable()
        {
            GameManager.OnStartGame += ToggleGameStatus;
            GameManager.OnStartGame += StartMalusCounter;
            GameManager.OnEndGame += ToggleGameStatus;
        }

        void Update()
        {
            if (timerStarted)
            {
                UpdateLevelTimer();
            }
        }

        private void UpdateLevelTimer()
        {
            elapsedTime += 1 * Time.deltaTime;

            TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
            gameTimer.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);

        }

        public void ToggleGameStatus()
        {
            timerStarted = !timerStarted;
            if(!timerStarted)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        public void StartMalusCounter()
        {
            InvokeRepeating("ApplyMalus", 10f, 10f);
        }

        private void ApplyMalus()
        {
            GameManager.Singleton.AddPointsToScore(-2);
        }

        private void OnDisable()
        {
            GameManager.OnStartGame -= ToggleGameStatus;
            GameManager.OnStartGame -= StartMalusCounter;
            GameManager.OnEndGame -= ToggleGameStatus;
        }

    }
}
