using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Walks to a firing line, then shoots at the castle on a timer.
    /// </summary>
    [RequireComponent(typeof(EnemyMovement))]
    public class RangedEnemyAttack : MonoBehaviour
    {
        [Tooltip("World Z where this enemy stops and starts shooting. Smaller values are closer to the castle.")]
        [SerializeField] private float stopZ = 3f;

        [SerializeField] private float fireInterval = 2.5f;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;

        private static readonly int IdleHash = Animator.StringToHash("Idle");

        private EnemyMovement movement;
        private Animator animator;
        private bool stopped;
        private float cooldown;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            animator = GetComponentInChildren<Animator>();
            cooldown = fireInterval;
        }

        private void Update()
        {
            if (!stopped)
            {
                if (transform.position.z > stopZ)
                {
                    return;
                }

                stopped = true;
                movement.HoldPosition();
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }

                if (animator != null)
                {
                    animator.CrossFade(IdleHash, 0.15f);
                }
            }

            cooldown -= Time.deltaTime;
            if (cooldown > 0f)
            {
                return;
            }

            cooldown = fireInterval;
            Fire();
        }

        private void Fire()
        {
            if (projectilePrefab == null)
            {
                return;
            }

            Vector3 origin = firePoint != null ? firePoint.position : transform.position + Vector3.up;
            Instantiate(projectilePrefab, origin, Quaternion.LookRotation(Vector3.back));
        }

        private void OnValidate()
        {
            fireInterval = Mathf.Max(0.1f, fireInterval);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.45f, 0.2f);
            Gizmos.DrawLine(new Vector3(-12f, 1f, stopZ), new Vector3(12f, 1f, stopZ));
        }
    }
}
