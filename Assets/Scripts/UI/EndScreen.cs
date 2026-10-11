using UnityEngine;
using TMPro;

namespace CastleArcher
{
    /// <summary>
    /// Shows the game-over or victory panel. The buttons only ask the game state to change scene.
    /// </summary>
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private GameStateController gameState;
        [SerializeField] private ScoreTracker score;
        [SerializeField] private GameObject gameOverRoot;
        [SerializeField] private GameObject victoryRoot;
        [SerializeField] private TMP_Text gameOverScore;
        [SerializeField] private TMP_Text victoryScore;

        private void Awake()
        {
            if (gameState == null)
            {
                gameState = FindAnyObjectByType<GameStateController>();
            }

            if (score == null)
            {
                score = FindAnyObjectByType<ScoreTracker>();
            }
        }

        private void OnEnable()
        {
            if (gameState != null)
            {
                gameState.StateChanged += Show;
            }
        }

        private void Start()
        {
            Show(gameState != null ? gameState.State : GameState.Playing);
        }

        private void OnDisable()
        {
            if (gameState != null)
            {
                gameState.StateChanged -= Show;
            }
        }

        public void Retry()
        {
            if (gameState != null)
            {
                gameState.Restart();
            }
        }

        public void ReturnToMenu()
        {
            if (gameState != null)
            {
                gameState.ReturnToMenu();
            }
        }

        private void Show(GameState state)
        {
            bool over = state == GameState.GameOver;
            bool victory = state == GameState.Victory;
            if (gameOverRoot != null)
            {
                gameOverRoot.SetActive(over);
            }

            if (victoryRoot != null)
            {
                victoryRoot.SetActive(victory);
            }

            if (!over && !victory)
            {
                return;
            }

            string text = score != null ? $"Score {score.Score}" : "Score 0";
            if (gameOverScore != null)
            {
                gameOverScore.text = text;
            }

            if (victoryScore != null)
            {
                victoryScore.text = text;
            }
        }
    }
}
