using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CastleArcher
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Victory
    }

    /// <summary>
    /// Playing until the castle falls or the last wave is cleared, then a paused end state.
    /// Restart reloads this scene at normal speed.
    /// </summary>
    public class GameStateController : MonoBehaviour
    {
        [Tooltip("Left empty, the castle is found on the Castle layer when play starts.")]
        [SerializeField] private Health castle;

        [SerializeField] private WaveRunner waves;

        private GameState state = GameState.Playing;
        private bool paused;

        public GameState State => state;

        public event Action<GameState> StateChanged;

        private void Awake()
        {
            if (waves == null)
            {
                waves = GetComponent<WaveRunner>();
            }

            if (castle == null)
            {
                castle = FindCastle();
            }
        }

        private void OnEnable()
        {
            if (castle != null)
            {
                castle.OnDied += HandleCastleDied;
            }

            if (waves != null)
            {
                waves.OnAllWavesCleared += HandleAllWavesCleared;
            }
        }

        private void Start()
        {
            StateChanged?.Invoke(state);
            Debug.Log("Playing.", this);
        }

        private void OnDisable()
        {
            if (castle != null)
            {
                castle.OnDied -= HandleCastleDied;
            }

            if (waves != null)
            {
                waves.OnAllWavesCleared -= HandleAllWavesCleared;
            }
        }

        private void OnDestroy()
        {
            if (paused)
            {
                Time.timeScale = 1f;
            }
        }

        /// <summary>Loads the current scene. Time scale is restored first so the new scene is not frozen.</summary>
        public void Restart()
        {
            Load("Gameplay");
        }

        /// <summary>Returns to the main menu. Time scale is restored first so the menu is not frozen.</summary>
        public void ReturnToMenu()
        {
            Load("MainMenu");
        }

        private void Load(string sceneName)
        {
            Time.timeScale = 1f;
            paused = false;
            SceneManager.LoadScene(sceneName);
        }

        private void HandleCastleDied()
        {
            End(GameState.GameOver, "Game over.");
        }

        private void HandleAllWavesCleared()
        {
            End(GameState.Victory, "Victory.");
        }

        private void End(GameState next, string message)
        {
            if (state != GameState.Playing)
            {
                return;
            }

            state = next;
            paused = true;
            Time.timeScale = 0f;
            StateChanged?.Invoke(state);
            Debug.Log(message, this);
        }

        private static Health FindCastle()
        {
            int castleLayer = LayerMask.NameToLayer("Castle");
            Health[] pools = FindObjectsByType<Health>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i].gameObject.layer == castleLayer)
                {
                    return pools[i];
                }
            }

            return null;
        }
    }
}
