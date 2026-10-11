using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Slides in along the ground with the enemies. If the archer is close, it flies to him and is collected.
    /// Otherwise it stays on the ground and passes the wall.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PowerupPickup : MonoBehaviour
    {
        [SerializeField] private PowerupDefinition definition;
        [SerializeField] private GameObject glowPrefab;
        [SerializeField] private float glowScale = 0.35f;

        [Tooltip("World Z where the archer can collect it. In front of the wall, so the flight is still on screen.")]
        [SerializeField] private float collectZ = -1f;

        [Tooltip("How far the archer may stand to either side and still collect it.")]
        [SerializeField] private float collectRadius = 3f;

        [Tooltip("Speed of the flight from the ground up to the archer.")]
        [SerializeField] private float flySpeed = 18f;

        private bool collected;
        private bool flying;
        private Rigidbody body;
        private EnemyMovement movement;
        private PowerupCollector collector;

        /// <summary>Used by the spawner so the instance matches the asset that created it.</summary>
        public void Apply(PowerupDefinition value)
        {
            definition = value;
            Tint();
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            movement = GetComponent<EnemyMovement>();
            collector = FindAnyObjectByType<PowerupCollector>();
            Tint();
            if (glowPrefab == null)
            {
                return;
            }

            GameObject glow = Instantiate(glowPrefab, transform);
            glow.transform.localPosition = Vector3.up * 0.4f;
            glow.transform.localScale = Vector3.one * glowScale;
        }

        private void FixedUpdate()
        {
            if (collected)
            {
                return;
            }

            if (flying)
            {
                FlyToArcher();
                return;
            }

            if (collector == null || definition == null)
            {
                return;
            }

            if (transform.position.z > collectZ)
            {
                return;
            }

            float side = Mathf.Abs(collector.transform.position.x - transform.position.x);
            if (side > collectRadius)
            {
                return;
            }

            flying = true;
            if (movement != null)
            {
                movement.HoldPosition();
            }

            FlyToArcher();
        }

        private void FlyToArcher()
        {
            Vector3 target = collector.transform.position;
            Vector3 next = Vector3.MoveTowards(body.position, target, flySpeed * Time.fixedDeltaTime);
            body.MovePosition(next);
            if ((target - next).sqrMagnitude > 0.04f)
            {
                return;
            }

            collected = true;
            collector.Collect(definition);
            Destroy(gameObject);
        }

        private void Tint()
        {
            if (definition == null)
            {
                return;
            }

            // _BaseColor on the potion replaces only the orange liquid. The grey neck stays.
            var block = new MaterialPropertyBlock();
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetPropertyBlock(block);
                block.SetColor("_BaseColor", definition.Color);
                block.SetColor("_Color", definition.Color);
                renderers[i].SetPropertyBlock(block);
            }
        }

        private void OnValidate()
        {
            collectRadius = Mathf.Max(0.1f, collectRadius);
            flySpeed = Mathf.Max(0.1f, flySpeed);
        }
    }
}
