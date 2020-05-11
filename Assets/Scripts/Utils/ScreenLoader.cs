using UnityEngine;
using UnityEngine.SceneManagement;

namespace Klondike.Utils
{
    public class ScreenLoader : MonoBehaviour
    {

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

    } 
}


