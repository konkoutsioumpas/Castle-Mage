using System;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// One entry in a wave: how many of one enemy to spawn.
    /// </summary>
    [Serializable]
    public struct WaveSpawn
    {
        public EnemyDefinition enemy;
        public int count;
    }

    /// <summary>
    /// The contents and pace of a single wave.
    /// </summary>
    [CreateAssetMenu(fileName = "Wave", menuName = "Castle Archer/Wave Definition")]
    public class WaveDefinition : ScriptableObject
    {
        [SerializeField] private float spawnInterval = 1f;
        [SerializeField] private WaveSpawn[] spawns;

        public float SpawnInterval => spawnInterval;
        public WaveSpawn[] Spawns => spawns;

        private void OnValidate()
        {
            spawnInterval = Mathf.Max(0.05f, spawnInterval);
            if (spawns == null)
            {
                return;
            }

            for (int i = 0; i < spawns.Length; i++)
            {
                WaveSpawn group = spawns[i];
                group.count = Mathf.Max(1, group.count);
                spawns[i] = group;
            }
        }
    }
}
