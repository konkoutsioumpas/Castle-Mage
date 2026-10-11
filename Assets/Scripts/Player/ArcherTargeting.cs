using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Picks the enemy the archer is allowed to shoot: inside a range and a forward cone.
    /// </summary>
    /// <remarks>
    /// The root stays facing the field. The mesh yaws with movement, but this cone uses
    /// the root, so walking sideways still picks whoever is in front. Only the fire point
    /// aims, so arrows leave toward the chosen enemy.
    /// </remarks>
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(ArcherShooter))]
    public class ArcherTargeting : MonoBehaviour
    {
        [SerializeField] private float range = 18f;

        [Tooltip("Full angle of the firing arc, in degrees, measured on the ground. An enemy below the bow still counts as in front.")]
        [SerializeField] private float coneAngle = 50f;

        [SerializeField] private float scanInterval = 0.1f;

        [Tooltip("Metres above the enemy root to aim at, so the arrow meets the body rather than the feet.")]
        [SerializeField] private float aimHeight = 1f;

        [SerializeField] private LayerMask enemyLayers;

        private readonly Collider[] hits = new Collider[32];
        private ArcherShooter shooter;
        private float scanTimer;

        /// <summary>The enemy currently inside the cone and closest to the castle. Null when nobody qualifies.</summary>
        public Transform CurrentTarget { get; private set; }

        private void Awake()
        {
            shooter = GetComponent<ArcherShooter>();

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayers.value == 0 && enemyLayer >= 0)
            {
                enemyLayers = 1 << enemyLayer;
            }
        }

        private void Update()
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer <= 0f)
            {
                scanTimer = scanInterval;
                CurrentTarget = FindTarget();
                shooter.CurrentTarget = CurrentTarget;
            }

            AimBow();
        }

        private Transform FindTarget()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
            {
                return null;
            }

            forward.Normalize();
            float halfAngle = coneAngle * 0.5f;
            int count = Physics.OverlapSphereNonAlloc(transform.position, range, hits, enemyLayers);

            Transform closest = null;
            float closestZ = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                Transform enemy = EnemyRoot(hits[i]);
                if (enemy == null || IsDead(enemy))
                {
                    continue;
                }

                Vector3 toEnemy = enemy.position - transform.position;
                toEnemy.y = 0f;
                if (toEnemy.sqrMagnitude < 0.0001f)
                {
                    continue;
                }

                if (Vector3.Angle(forward, toEnemy) > halfAngle)
                {
                    continue;
                }

                if (enemy.position.z >= closestZ)
                {
                    continue;
                }

                closestZ = enemy.position.z;
                closest = enemy;
            }

            return closest;
        }

        private void AimBow()
        {
            Transform bow = shooter.FirePoint;
            if (bow == null)
            {
                return;
            }

            // Destroyed enemies compare equal to null, so a kill cannot leave the bow aimed at nothing.
            if (CurrentTarget == null)
            {
                shooter.CurrentTarget = null;
                bow.localRotation = Quaternion.identity;
                return;
            }

            bow.LookAt(CurrentTarget.position + Vector3.up * aimHeight);
        }

        private static Transform EnemyRoot(Collider hit)
        {
            if (hit == null)
            {
                return null;
            }

            return hit.attachedRigidbody != null ? hit.attachedRigidbody.transform : hit.transform;
        }

        private static bool IsDead(Transform enemy)
        {
            Health health = enemy.GetComponent<Health>();
            return health != null && health.IsDead;
        }

        private void OnValidate()
        {
            range = Mathf.Max(0.1f, range);
            coneAngle = Mathf.Clamp(coneAngle, 1f, 180f);
            scanInterval = Mathf.Max(0.02f, scanInterval);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
            {
                return;
            }

            forward.Normalize();
            float halfAngle = coneAngle * 0.5f;

            Gizmos.color = new Color(0.2f, 0.85f, 1f);
            DrawCircle(origin, range);
            Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, -halfAngle, 0f) * forward * range);
            Gizmos.DrawLine(origin, origin + Quaternion.Euler(0f, halfAngle, 0f) * forward * range);

            if (CurrentTarget == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, CurrentTarget.position + Vector3.up * aimHeight);
        }

        private static void DrawCircle(Vector3 center, float radius)
        {
            const int segments = 48;
            Vector3 previous = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }
    }
}
