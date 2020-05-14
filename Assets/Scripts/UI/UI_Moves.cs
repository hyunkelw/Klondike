using UnityEngine;
using TMPro;
using Klondike.Game;

namespace Klondike.UI
{
    public class UI_Moves : MonoBehaviour
    {
        /*Reference to moves counter text */
        [SerializeField] private TextMeshProUGUI movesCounter = default;

        public void UpdateMovesCounter()
        {
            if (!movesCounter)
            {
                Debug.LogWarning("[UI_Moves] Moves Counter not setted!");
                return;
            }
            movesCounter.text = GameManager.Singleton.Moves.ToString();
        }
    }
}