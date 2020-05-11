using UnityEngine;
using TMPro;
using Klondike.Game;

namespace Klondike.UI
{

    public class UI_Moves : MonoBehaviour
    {
        /*Reference to moves counter text */
        [SerializeField] private TextMeshProUGUI movesCounter = default;


        private void OnEnable()
        {
            GameManager.OnValidMove += UpdateMovesCounter;
        }

        public void UpdateMovesCounter()
        {
            if (!movesCounter)
            {
                Debug.LogWarning("[MovesDisplay] Moves Counter not setted!");
                return;
            }
            movesCounter.text = GameManager.Singleton.Moves.ToString();
        }

        private void OnDestroy()
        {
            GameManager.OnValidMove -= UpdateMovesCounter;

        }
    }
}