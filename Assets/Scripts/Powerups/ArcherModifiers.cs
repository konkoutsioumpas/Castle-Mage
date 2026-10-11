using System;
using System.Collections;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Turns the archer's fire rate and arrow count up for a limited time, then puts them back.
    /// </summary>
    /// <remarks>
    /// Picking up the same buff again restarts its timer. It does not multiply the bonus a
    /// second time. Rapid Fire and Multi-Shot each keep their own timer, so one does not cancel the other.
    /// </remarks>
    [RequireComponent(typeof(ArcherShooter))]
    public class ArcherModifiers : MonoBehaviour
    {
        [SerializeField] private float rapidFireMultiplier = 2f;
        [SerializeField] private int multiShotArrows = 3;

        private ArcherShooter shooter;
        private Coroutine rapidFireRoutine;
        private Coroutine multiShotRoutine;
        private float rapidFireRemaining;
        private float multiShotRemaining;

        /// <summary>What the HUD should show. Empty when no timed buff is active.</summary>
        public string BuffLabel { get; private set; } = string.Empty;

        /// <summary>Raised when a timed buff starts, ticks, or wears off.</summary>
        public event Action<string> OnBuffsChanged;

        private void Awake()
        {
            shooter = GetComponent<ArcherShooter>();
        }

        /// <summary>Starts or refreshes the buff matching the definition. Castle Repair is ignored here.</summary>
        public void Apply(PowerupDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            switch (definition.Type)
            {
                case PowerupType.RapidFire:
                    rapidFireRoutine = Restart(rapidFireRoutine, RapidFire(definition.Duration));
                    break;
                case PowerupType.MultiShot:
                    multiShotRoutine = Restart(multiShotRoutine, MultiShot(definition.Duration));
                    break;
            }
        }

        private Coroutine Restart(Coroutine running, IEnumerator effect)
        {
            if (running != null)
            {
                StopCoroutine(running);
            }

            return StartCoroutine(effect);
        }

        private IEnumerator RapidFire(float duration)
        {
            shooter.FireRateMultiplier = rapidFireMultiplier;
            Debug.Log($"Rapid Fire for {duration:0.#} seconds.", this);
            yield return Tick(duration, value => rapidFireRemaining = value);
            shooter.FireRateMultiplier = 1f;
            rapidFireRemaining = 0f;
            rapidFireRoutine = null;
            PublishBuffs();
            Debug.Log("Rapid Fire wore off.", this);
        }

        private IEnumerator MultiShot(float duration)
        {
            shooter.ArrowsPerShot = Mathf.Max(1, multiShotArrows);
            Debug.Log($"Multi-Shot for {duration:0.#} seconds.", this);
            yield return Tick(duration, value => multiShotRemaining = value);
            shooter.ArrowsPerShot = 1;
            multiShotRemaining = 0f;
            multiShotRoutine = null;
            PublishBuffs();
            Debug.Log("Multi-Shot wore off.", this);
        }

        private IEnumerator Tick(float duration, Action<float> store)
        {
            float remaining = duration;
            while (remaining > 0f)
            {
                store(remaining);
                PublishBuffs();
                float step = Mathf.Min(0.25f, remaining);
                yield return new WaitForSeconds(step);
                remaining = Mathf.Max(0f, remaining - step);
            }

            store(0f);
        }

        private void PublishBuffs()
        {
            BuffLabel = BuildLabel();
            OnBuffsChanged?.Invoke(BuffLabel);
        }

        private string BuildLabel()
        {
            bool rapid = rapidFireRemaining > 0f;
            bool multi = multiShotRemaining > 0f;
            if (rapid && multi)
            {
                return $"Rapid Fire {Mathf.CeilToInt(rapidFireRemaining)}s    Multi-Shot {Mathf.CeilToInt(multiShotRemaining)}s";
            }

            if (rapid)
            {
                return $"Rapid Fire {Mathf.CeilToInt(rapidFireRemaining)}s";
            }

            if (multi)
            {
                return $"Multi-Shot {Mathf.CeilToInt(multiShotRemaining)}s";
            }

            return string.Empty;
        }

        private void OnDisable()
        {
            if (shooter == null)
            {
                return;
            }

            shooter.FireRateMultiplier = 1f;
            shooter.ArrowsPerShot = 1;
            rapidFireRemaining = 0f;
            multiShotRemaining = 0f;
            BuffLabel = string.Empty;
        }

        private void OnValidate()
        {
            rapidFireMultiplier = Mathf.Max(1f, rapidFireMultiplier);
            multiShotArrows = Mathf.Max(2, multiShotArrows);
        }
    }
}
