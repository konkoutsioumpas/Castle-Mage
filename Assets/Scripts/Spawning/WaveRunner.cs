using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Plays a fixed list of waves. The next wave starts only after every enemy from the current one is gone.
    /// </summary>
    public class WaveRunner : MonoBehaviour
    {
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private WaveDefinition[] waves;
        [SerializeField] private float startDelay = 1f;
        [SerializeField] private float restDuration = 2f;

        private readonly List<GameObject> alive = new List<GameObject>();
        private Coroutine routine;

        /// <summary>1-based wave currently being played. Zero until the first wave starts.</summary>
        public int CurrentWave { get; private set; }

        public int WaveCount => waves == null ? 0 : waves.Length;

        /// <summary>Raised with a 1-based wave number when that wave begins spawning.</summary>
        public event Action<int> OnWaveStarted;

        /// <summary>Raised once, after the last wave has been cleared.</summary>
        public event Action OnAllWavesCleared;

        private void OnEnable()
        {
            routine = StartCoroutine(RunWaves());
        }

        private void OnDisable()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
        }

        private IEnumerator RunWaves()
        {
            yield return new WaitForSeconds(startDelay);

            if (waves == null || waves.Length == 0 || spawner == null)
            {
                Debug.LogWarning("WaveRunner has no waves or no spawner.", this);
                yield break;
            }

            for (int i = 0; i < waves.Length; i++)
            {
                int number = i + 1;
                CurrentWave = number;
                OnWaveStarted?.Invoke(number);
                Debug.Log($"Wave {number} started.", this);
                yield return SpawnWave(waves[i]);
                yield return new WaitUntil(WaveIsClear);
                Debug.Log($"Wave {number} cleared.", this);

                if (i < waves.Length - 1 && restDuration > 0f)
                {
                    yield return new WaitForSeconds(restDuration);
                }
            }

            OnAllWavesCleared?.Invoke();
            Debug.Log("All waves cleared.", this);
        }

        private IEnumerator SpawnWave(WaveDefinition wave)
        {
            if (wave == null || wave.Spawns == null)
            {
                yield break;
            }

            for (int i = 0; i < wave.Spawns.Length; i++)
            {
                WaveSpawn group = wave.Spawns[i];
                for (int n = 0; n < group.count; n++)
                {
                    GameObject enemy = spawner.Spawn(group.enemy);
                    if (enemy != null)
                    {
                        alive.Add(enemy);
                    }

                    if (n < group.count - 1 || i < wave.Spawns.Length - 1)
                    {
                        yield return new WaitForSeconds(wave.SpawnInterval);
                    }
                }
            }
        }

        private bool WaveIsClear()
        {
            for (int i = alive.Count - 1; i >= 0; i--)
            {
                if (alive[i] == null)
                {
                    alive.RemoveAt(i);
                }
            }

            return alive.Count == 0;
        }

        private void OnValidate()
        {
            startDelay = Mathf.Max(0f, startDelay);
            restDuration = Mathf.Max(0f, restDuration);
        }
    }
}
