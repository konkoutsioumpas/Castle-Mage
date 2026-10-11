using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace CastleArcher
{
    /// <summary>
    /// Flies forward, damages the first valid target it touches, then removes itself.
    /// </summary>
    /// <remarks>
    /// The archer's arrows and later enemy shots share this component. Speed, damage,
    /// and <see cref="hitLayers"/> are the only differences between them.
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private int damage = 10;

        [Tooltip("Seconds before a missed shot is removed, so it cannot fly forever.")]
        [SerializeField] private float lifetime = 3f;

        [Tooltip("Layers this projectile can hit. Archer arrows use Enemy.")]
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private AudioClip impactClip;
        [SerializeField] private float impactVolume = 0.55f;
        [SerializeField] private GameObject impactEffect;
        [SerializeField] private float impactEffectLifetime = 1.5f;

        private Rigidbody body;
        private bool hasStruck;
        private bool despawned;
        private Coroutine life;
        private ObjectPool<Projectile> pool;

        /// <summary>Called by the archer's pool before this arrow is activated.</summary>
        public void AssignPool(ObjectPool<Projectile> arrowPool)
        {
            pool = arrowPool;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (hitLayers.value == 0 && enemyLayer >= 0)
            {
                hitLayers = 1 << enemyLayer;
            }
        }

        private void OnEnable()
        {
            hasStruck = false;
            despawned = false;
            life = StartCoroutine(Expire());
        }

        private void OnDisable()
        {
            if (life != null)
            {
                StopCoroutine(life);
                life = null;
            }
        }

        private IEnumerator Expire()
        {
            yield return new WaitForSeconds(lifetime);
            Despawn();
        }

        private void FixedUpdate()
        {
            if (hasStruck)
            {
                return;
            }

            float step = speed * Time.fixedDeltaTime;
            body.MovePosition(body.position + transform.forward * step);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasStruck || !IsTarget(other.gameObject))
            {
                return;
            }

            hasStruck = true;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Component speaker = other.GetComponentInParent<Health>();
                OneShot.Play(speaker != null ? speaker : other, impactClip, impactVolume);
                SpawnImpact(other.ClosestPoint(transform.position));
            }

            Despawn();
        }

        private void Despawn()
        {
            if (despawned)
            {
                return;
            }

            despawned = true;
            if (pool != null)
            {
                pool.Release(this);
                return;
            }

            Destroy(gameObject);
        }

        private void SpawnImpact(Vector3 point)
        {
            if (impactEffect == null)
            {
                return;
            }

            GameObject effect = Instantiate(impactEffect, point, Quaternion.identity);
            Destroy(effect, impactEffectLifetime);
        }

        private bool IsTarget(GameObject target)
        {
            return (hitLayers.value & (1 << target.layer)) != 0;
        }

        private void OnValidate()
        {
            speed = Mathf.Max(0f, speed);
            damage = Mathf.Max(0, damage);
            lifetime = Mathf.Max(0.1f, lifetime);
        }
    }
}
