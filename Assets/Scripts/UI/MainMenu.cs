using UnityEngine;
using UnityEngine.SceneManagement;

namespace CastleArcher
{
    /// <summary>
    /// Play starts a fresh game. Quit is hidden in WebGL, where it does nothing.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject quitButton;

        private void Awake()
        {
            Time.timeScale = 1f;
            if (quitButton != null && Application.platform == RuntimePlatform.WebGLPlayer)
            {
                quitButton.SetActive(false);
            }
        }

        public void Play()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Gameplay");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
