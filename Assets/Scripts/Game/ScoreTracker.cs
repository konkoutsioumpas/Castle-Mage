using System;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Running score for this session, plus a high score kept in PlayerPrefs.
    /// </summary>
    public class ScoreTracker : MonoBehaviour
    {
        private const string HighScoreKey = "CastleArcher.HighScore";

        private int score;

        public int Score => score;
        public int HighScore { get; private set; }

        /// <summary>Raised with the new total after a kill is awarded.</summary>
        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        /// <summary>Adds a kill's score value. Zero and negative amounts are ignored.</summary>
        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            score += amount;
            if (score > HighScore)
            {
                HighScore = score;
                PlayerPrefs.SetInt(HighScoreKey, HighScore);
            }

            OnScoreChanged?.Invoke(score);
            Debug.Log($"Score +{amount} -> {score} (high {HighScore}).", this);
        }
    }
}
