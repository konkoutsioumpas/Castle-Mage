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
            Debug.Log($"{name} reached 0 health.", this);
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
