using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Moves an object toward the castle at a constant speed.
    /// </summary>
    /// <remarks>
    /// Used by enemies and by power-ups. The body is kinematic: a tower-defence
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

        /// <summary>Replaces the prefab speed. Spawn definitions call this after the object exists.</summary>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = Mathf.Max(0f, speed);
        }

        private bool holdPosition;

        /// <summary>Stops the advance. A ranged enemy calls this once it has reached its firing line.</summary>
        public void HoldPosition()
        {
            holdPosition = true;
        }

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

            if (holdPosition)
            {
                return;
            }

            float step = moveSpeed * SpeedMultiplier * Time.fixedDeltaTime;
            body.MovePosition(body.position + Vector3.back * step);
        }
    }
}
