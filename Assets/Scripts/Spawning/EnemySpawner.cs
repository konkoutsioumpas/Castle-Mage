using System.Collections;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Spawns enemies and power-ups from their definitions.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private PowerupDefinition[] powerups;

        [SerializeField] private float spawnZ = 13f;
        [SerializeField] private float spawnXRange = 10f;
        [SerializeField] private float spawnY = 0.75f;
        [SerializeField] private float powerupXRange = 9f;

        [SerializeField] private float startDelay = 1f;
        [SerializeField] private float powerupSpawnInterval = 5f;

        private Coroutine powerupRoutine;

        private void OnEnable()
        {
            powerupRoutine = StartCoroutine(SpawnPowerups());
        }

        private void OnDisable()
        {
            if (powerupRoutine != null)
            {
                StopCoroutine(powerupRoutine);
            }
        }

        private IEnumerator SpawnPowerups()
        {
            yield return new WaitForSeconds(startDelay);

            while (true)
            {
                SpawnPowerup();
                yield return new WaitForSeconds(powerupSpawnInterval);
            }
        }

        /// <summary>Spawns one enemy from a wave entry. Returns null when the definition has no prefab.</summary>
        public GameObject Spawn(EnemyDefinition definition)
        {
            if (definition == null || definition.Prefab == null)
            {
                return null;
            }

            GameObject enemy = Instantiate(definition.Prefab, SpawnPosition(definition.SpawnHeight), definition.Prefab.transform.rotation);
            Apply(enemy, definition);
            return enemy;
        }

        private void SpawnPowerup()
        {
            PowerupDefinition definition = Choose(powerups);
            if (definition == null || definition.Prefab == null)
            {
                return;
            }

            Vector3 position = new Vector3(Random.Range(-powerupXRange, powerupXRange), spawnY, spawnZ);
            GameObject pickupObject = Instantiate(definition.Prefab, position, Quaternion.identity);
            PowerupPickup pickup = pickupObject.GetComponent<PowerupPickup>();
            if (pickup != null)
            {
                pickup.Apply(definition);
            }
        }

        private Vector3 SpawnPosition(float height)
        {
            return new Vector3(Random.Range(-spawnXRange, spawnXRange), height, spawnZ);
        }

        private static T Choose<T>(T[] options) where T : class
        {
            if (options == null || options.Length == 0)
            {
                return null;
            }

            return options[Random.Range(0, options.Length)];
        }

        private static void Apply(GameObject enemy, EnemyDefinition definition)
        {
            Health health = enemy.GetComponent<Health>();
            if (health != null)
            {
                health.SetMaxHealth(definition.MaxHealth);
            }

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetMoveSpeed(definition.MoveSpeed);
            }

            EnemyAttack attack = enemy.GetComponent<EnemyAttack>();
            if (attack != null)
            {
                attack.SetCastleDamage(definition.CastleDamage);
            }

            EnemyDeathHandler death = enemy.GetComponent<EnemyDeathHandler>();
            if (death != null)
            {
                death.SetScoreValue(definition.ScoreValue);
            }
        }

        private void OnValidate()
        {
            spawnXRange = Mathf.Max(0f, spawnXRange);
            startDelay = Mathf.Max(0f, startDelay);
            powerupXRange = Mathf.Max(0f, powerupXRange);
            powerupSpawnInterval = Mathf.Max(0.05f, powerupSpawnInterval);
        }
    }
}
