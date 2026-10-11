using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Receives power-ups the archer walks into. Timed buffs go to <see cref="ArcherModifiers"/>.
    /// Castle Repair heals the castle.
    /// </summary>
    [RequireComponent(typeof(ArcherModifiers))]
    public class PowerupCollector : MonoBehaviour
    {
        [Tooltip("Left empty, the castle is found on the Castle layer when play starts.")]
        [SerializeField] private Health castle;

        [SerializeField] private int repairAmount = 25;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private float pickupVolume = 0.6f;

        private ArcherModifiers modifiers;

        private void Awake()
        {
            modifiers = GetComponent<ArcherModifiers>();
            if (castle == null)
            {
                castle = FindCastle();
            }
        }

        /// <summary>Called by a pickup.</summary>
        public void Collect(PowerupDefinition definition)
        {
            if (definition == null)
            {
                return;
            }

            Debug.Log($"Collected {definition.DisplayName}.", this);
            OneShot.Play(this, pickupClip, pickupVolume);

            switch (definition.Type)
            {
                case PowerupType.RapidFire:
                case PowerupType.MultiShot:
                    modifiers.Apply(definition);
                    break;
                case PowerupType.CastleRepair:
                    RepairCastle();
                    break;
            }
        }

        private void RepairCastle()
        {
            if (castle == null)
            {
                Debug.LogWarning("Castle Repair could not find the castle health.", this);
                return;
            }

            int before = castle.CurrentHealth;
            castle.Heal(repairAmount);
            Debug.Log($"Castle health {before} -> {castle.CurrentHealth} (max {castle.MaxHealth}).", castle);
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

        private void OnValidate()
        {
            repairAmount = Mathf.Max(1, repairAmount);
        }
    }
}
