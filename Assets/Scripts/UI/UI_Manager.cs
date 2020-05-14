using System;
using Klondike.Game;
using UnityEngine;

namespace Klondike.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] GameObject winPanel = default;

        void OnEnable()
        {
            GameManager.OnValidMove += UpdateMovesCounter;
            GameManager.OnValidMove += UpdateTimerText;
            GameManager.OnScoreUpdated += UpdateScoreCounter;
            GameManager.OnEndGame += ShowWinPanel;
        }

        public void StartNewGameButton()
        {
            FindObjectOfType<GameManager>().StartNewGame();
        }

        public void RestartGameButton()
        {
            FindObjectOfType<GameManager>().ReplayGame();
        }

        public void UndoButton()
        {
            FindObjectOfType<GameManager>().UndoMove();
        }


        private void OnDisable()
        {
            GameManager.OnValidMove -= UpdateMovesCounter;
            GameManager.OnValidMove -= UpdateTimerText;
            GameManager.OnScoreUpdated -= UpdateScoreCounter;
            GameManager.OnEndGame -= ShowWinPanel;

        }

        private void ShowWinPanel()
        {
            winPanel.SetActive(true);
        }

        public void UpdateTimerText()
        {
            UI_Timer[] ui_timers = GetComponentsInChildren<UI_Timer>();
            foreach (var ui_timerComponent in ui_timers)
            {
                ui_timerComponent.UpdateTimerText();
            }
        }

        public void UpdateMovesCounter()
        {
            UI_Moves[] ui_moveCounters = GetComponentsInChildren<UI_Moves>();
            foreach (var ui_movesComponent in ui_moveCounters)
            {
                ui_movesComponent.UpdateMovesCounter();
            }
        }

        public void UpdateScoreCounter()
        {
            UI_Score[] ui_scores = GetComponentsInChildren<UI_Score>();
            foreach (var ui_scoreComponent in ui_scores)
            {
                ui_scoreComponent.UpdateScoreCounter();
            }
        }


        public void QuitGameButton()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }


    }
}