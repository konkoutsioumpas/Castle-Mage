using UnityEngine;
using UnityEngine.Pool;

namespace CastleArcher
{
    /// <summary>
    /// Fires arrows on a timer from a fire point.
    /// </summary>
    /// <remarks>
    /// Targeting does not live here. <see cref="CurrentTarget"/> is assigned from outside,
    /// and <see cref="FireRateMultiplier"/> plus <see cref="ArrowsPerShot"/> are the only
    /// knobs a power-up needs.
    /// </remarks>
    public class ArcherShooter : MonoBehaviour
    {
        [SerializeField] private float shotsPerSecond = 2f;
        [SerializeField] private Projectile arrowPrefab;
        [SerializeField] private Transform firePoint;

        [Tooltip("Yaw between neighbouring arrows when more than one is fired.")]
        [SerializeField] private float spreadAngle = 8f;
        [SerializeField] private AudioClip shotClip;

        [Tooltip("Kept low so Rapid Fire overlaps do not clip.")]
        [SerializeField] private float shotVolume = 0.35f;

        private float cooldown;
        private Animator animator;
        private ObjectPool<Projectile> arrows;
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            if (arrowPrefab == null)
            {
                return;
            }

            arrows = new ObjectPool<Projectile>(
                CreateArrow,
                actionOnGet: arrow => arrow.gameObject.SetActive(true),
                actionOnRelease: arrow => arrow.gameObject.SetActive(false),
                actionOnDestroy: arrow => Destroy(arrow.gameObject),
                collectionCheck: false,
                defaultCapacity: 24,
                maxSize: 48);
        }

        private void OnDestroy()
        {
            arrows?.Dispose();
        }

        private Projectile CreateArrow()
        {
            Projectile arrow = Instantiate(arrowPrefab);
            arrow.gameObject.SetActive(false);
            arrow.AssignPool(arrows);
            return arrow;
        }

        /// <summary>What the archer is shooting at. Null means there is nothing to shoot.</summary>
        public Transform CurrentTarget { get; set; }

        /// <summary>Where arrows are spawned. Targeting aims this transform.</summary>
        public Transform FirePoint => firePoint;

        /// <summary>Scales <see cref="shotsPerSecond"/>. Rapid Fire changes this later.</summary>
        public float FireRateMultiplier { get; set; } = 1f;

        /// <summary>Arrows released together. Multi-Shot changes this. One arrow fires straight; extras spread around it.</summary>
        public int ArrowsPerShot { get; set; } = 1;

        private void Update()
        {
            if (arrowPrefab == null || firePoint == null)
            {
                return;
            }

            if (CurrentTarget == null)
            {
                return;
            }

            cooldown += Time.deltaTime;
            float interval = ShotInterval();
            if (cooldown < interval)
            {
                return;
            }

            cooldown -= interval;
            Fire();
        }

        private void Fire()
        {
            if (animator != null)
            {
                // Upper-body layer. The base layer stays on idle/run.
                animator.SetTrigger(AttackHash);
            }

            OneShot.Play(this, shotClip, shotVolume);

            int count = Mathf.Max(1, ArrowsPerShot);
            float start = -spreadAngle * (count - 1) * 0.5f;
            for (int i = 0; i < count; i++)
            {
                Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, start + spreadAngle * i, 0f);
                Projectile arrow = arrows.Get();
                arrow.transform.SetPositionAndRotation(firePoint.position, rotation);
            }
        }

        private float ShotInterval()
        {
            float shots = shotsPerSecond * Mathf.Max(0f, FireRateMultiplier);
            return shots > 0f ? 1f / shots : float.PositiveInfinity;
        }

        private void OnValidate()
        {
            shotsPerSecond = Mathf.Max(0f, shotsPerSecond);
        }
    }
}
