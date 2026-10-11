using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CastleArcher
{
    /// <summary>
    /// Displays castle health, score, wave, and timed buffs. It never changes gameplay.
    /// </summary>
    public class HudView : MonoBehaviour
    {
        [Tooltip("Left empty, the castle is found on the Castle layer when play starts.")]
        [SerializeField] private Health castle;

        [SerializeField] private ScoreTracker score;
        [SerializeField] private WaveRunner waves;
        [SerializeField] private ArcherModifiers modifiers;

        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthLabel;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text waveLabel;
        [SerializeField] private TMP_Text buffLabel;
        [SerializeField] private GameObject buffRoot;

        private void Awake()
        {
            if (castle == null)
            {
                castle = FindCastle();
            }

            if (score == null)
            {
                score = FindAnyObjectByType<ScoreTracker>();
            }

            if (waves == null)
            {
                waves = FindAnyObjectByType<WaveRunner>();
            }

            if (modifiers == null)
            {
                modifiers = FindAnyObjectByType<ArcherModifiers>();
            }
        }

        private void OnEnable()
        {
            if (castle != null)
            {
                castle.OnHealthChanged += ShowHealth;
            }

            if (score != null)
            {
                score.OnScoreChanged += ShowScore;
            }

            if (waves != null)
            {
                waves.OnWaveStarted += ShowWave;
            }

            if (modifiers != null)
            {
                modifiers.OnBuffsChanged += ShowBuffs;
            }
        }

        private void Start()
        {
            if (castle != null)
            {
                ShowHealth(castle.CurrentHealth, castle.MaxHealth);
            }

            if (score != null)
            {
                ShowScore(score.Score);
            }

            if (waves != null)
            {
                ShowWave(Mathf.Max(1, waves.CurrentWave));
            }

            ShowBuffs(modifiers != null ? modifiers.BuffLabel : string.Empty);
        }

        private void OnDisable()
        {
            if (castle != null)
            {
                castle.OnHealthChanged -= ShowHealth;
            }

            if (score != null)
            {
                score.OnScoreChanged -= ShowScore;
            }

            if (waves != null)
            {
                waves.OnWaveStarted -= ShowWave;
            }

            if (modifiers != null)
            {
                modifiers.OnBuffsChanged -= ShowBuffs;
            }
        }

        private void ShowHealth(int current, int max)
        {
            if (healthFill != null)
            {
                healthFill.fillAmount = max > 0 ? (float)current / max : 0f;
            }

            if (healthLabel != null)
            {
                healthLabel.text = $"{current} / {max}";
            }
        }

        private void ShowScore(int total)
        {
            if (scoreLabel != null)
            {
                scoreLabel.text = $"Score {total}";
            }
        }

        private void ShowWave(int number)
        {
            if (waveLabel == null)
            {
                return;
            }

            int count = waves != null ? waves.WaveCount : 0;
            waveLabel.text = count > 0 ? $"Wave {number} / {count}" : $"Wave {number}";
        }

        private void ShowBuffs(string label)
        {
            bool active = !string.IsNullOrEmpty(label);
            if (buffRoot != null)
            {
                buffRoot.SetActive(active);
            }

            if (buffLabel != null)
            {
                buffLabel.text = label;
            }
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
