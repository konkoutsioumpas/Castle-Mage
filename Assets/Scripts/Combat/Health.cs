using System;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// A hit-point pool. The same component is used for enemies and for the castle.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private int currentHealth;

        private bool isDead;

        /// <summary>Raised after every change, with (current, max).</summary>
        public event Action<int, int> OnHealthChanged;

        /// <summary>Raised once, the first time health reaches zero.</summary>
        public event Action OnDied;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => isDead;

        private void Awake()
        {
            currentHealth = maxHealth;
            isDead = false;
        }

        /// <summary>Replaces the pool. Used when a spawn definition supplies the real health.</summary>
        public void SetMaxHealth(int value)
        {
            if (isDead)
            {
                return;
            }

            maxHealth = Mathf.Max(1, value);
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (isDead || amount <= 0)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth > 0)
            {
                return;
            }

            isDead = true;
            OnDied?.Invoke();
        }

        public void Heal(int amount)
        {
            if (isDead || amount <= 0)
            {
                return;
            }

            int updated = Mathf.Min(maxHealth, currentHealth + amount);
            if (updated == currentHealth)
            {
                return;
            }

            currentHealth = updated;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            if (!Application.isPlaying)
            {
                currentHealth = maxHealth;
            }
        }

        [ContextMenu("Debug/Take 10 Damage")]
        private void DebugTakeDamage()
        {
            TakeDamage(10);
        }

        [ContextMenu("Debug/Heal 10")]
        private void DebugHeal()
        {
            Heal(10);
        }
    }
}
