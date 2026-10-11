using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// The stats for one kind of enemy. Waves and the spawner reference this, not the prefab.
    /// </summary>
    [CreateAssetMenu(fileName = "Enemy", menuName = "Castle Archer/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private int castleDamage = 10;

        [Tooltip("Awarded later by the score tracker. Stored here so a wave can read it without opening the prefab.")]
        [SerializeField] private int scoreValue = 10;

        [Tooltip("World Y used when this enemy is spawned.")]
        [SerializeField] private float spawnHeight = 0.75f;

        public GameObject Prefab => prefab;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public int CastleDamage => castleDamage;
        public int ScoreValue => scoreValue;
        public float SpawnHeight => spawnHeight;

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            moveSpeed = Mathf.Max(0f, moveSpeed);
            castleDamage = Mathf.Max(0, castleDamage);
            scoreValue = Mathf.Max(0, scoreValue);
        }
    }
}
