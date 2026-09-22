using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Moves an object toward the castle at a constant speed.
    /// </summary>
    /// <remarks>
    /// Used by enemies and by falling power-ups. The body is kinematic: a tower-defence
    /// mover needs an exact speed, and a kinematic body cannot be pushed off course by
    /// bumping into another one.
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;

        [Tooltip("World Z past which this object has gone behind the castle and is removed.")]
        [SerializeField] private float despawnZ = -16f;

        private Rigidbody body;

        /// <summary>Scales the configured speed. A slow or freeze effect can change this later without new movement code.</summary>
        public float SpeedMultiplier { get; set; } = 1f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
        }

        private void FixedUpdate()
        {
            if (transform.position.z < despawnZ)
            {
                Destroy(gameObject);
                return;
            }

            float step = moveSpeed * SpeedMultiplier * Time.fixedDeltaTime;
            body.MovePosition(body.position + Vector3.back * step);
        }
    }
}
