using Klondike.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Klondike.UI
{
    [RequireComponent(typeof(Button))]
    public class UI_UndoButton : MonoBehaviour
    {
        private Button button = default;

        private void OnEnable()
        {
            button = GetComponent<Button>();
            GameManager.OnValidMove += SetInteractable;
        }
        public void SetInteractable()
        {
            if (!button)
            {
                Debug.LogWarning("[UI_UndoButton] Button not setted!");
                return;
            }
            button.interactable = GameManager.Singleton.RevertableMoves > 0;
        }

        private void OnDisable()
        {
            GameManager.OnValidMove -= SetInteractable;
        }

    }

}