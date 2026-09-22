using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Damages the castle when this enemy reaches it, then removes the enemy.
    /// </summary>
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private int castleDamage = 10;

        [Tooltip("Layers that count as the castle. The front wall's trigger is on the Castle layer.")]
        [SerializeField] private LayerMask castleLayers;

        private bool hasStruck;

        private void Awake()
        {
            int castleLayer = LayerMask.NameToLayer("Castle");
            if (castleLayers.value == 0 && castleLayer >= 0)
            {
                castleLayers = 1 << castleLayer;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasStruck || !IsCastle(other.gameObject))
            {
                return;
            }

            IDamageable castle = other.GetComponentInParent<IDamageable>();
            if (castle == null)
            {
                return;
            }

            hasStruck = true;
            castle.TakeDamage(castleDamage);
            Destroy(gameObject);
        }

        private bool IsCastle(GameObject target)
        {
            return (castleLayers.value & (1 << target.layer)) != 0;
        }
    }
}
