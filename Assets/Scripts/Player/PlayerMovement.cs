using UnityEngine;
using UnityEngine.InputSystem;

namespace CastleArcher
{
    /// <summary>
    /// Moves the archer left and right along the rampart and keeps him inside the arena.
    /// </summary>
    /// <remarks>
    /// The root stays facing the field, so the firing cone does not swing. The mesh yaws
    /// to face the strafe while he is moving, and turns back when he stops. The run is
    /// the Blend parameter on the base layer: 0 is idle, 1 is running.
    /// </remarks>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;

        [Tooltip("Furthest the archer's centre may travel from x = 0. Sized to the camera view, not the walls: the side towers sit near ±14.7, but at the rampart the 16:9 frame only shows about ±10.")]
        [SerializeField] private float horizontalLimit = 10f;

        [Tooltip("How quickly Blend moves between idle (0) and run (1).")]
        [SerializeField] private float locomotionBlendTime = 0.12f;

        [Tooltip("How quickly the mesh yaws to face the strafe, or back toward the field when he stops.")]
        [SerializeField] private float turnSmoothTime = 0.15f;

        [SerializeField] private InputAction moveAction;

        private float horizontalInput;
        private float bodyYaw;
        private float yawVelocity;
        private Transform body;
        private Animator animator;
        private static readonly int BlendHash = Animator.StringToHash("Blend");

        /// <summary>Signed horizontal input, in the range -1 to 1.</summary>
        public float HorizontalInput => horizontalInput;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.applyRootMotion = false;
            body = animator.transform;

            ArcherHeadLook look = animator.GetComponent<ArcherHeadLook>();
            if (look == null)
            {
                look = animator.gameObject.AddComponent<ArcherHeadLook>();
            }

            look.Initialize(transform, GetComponent<ArcherTargeting>());
        }

        // The action is enabled for exactly as long as this component is active. Enabling it in
        // Start and never disabling it leaks the action whenever the scene is reloaded.
        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            if (animator != null)
            {
                animator.SetFloat(BlendHash, 0f);
            }

            bodyYaw = 0f;
            yawVelocity = 0f;
            if (body != null)
            {
                body.localRotation = Quaternion.identity;
            }
        }

        private void Update()
        {
            horizontalInput = moveAction.ReadValue<Vector2>().x;
            UpdateLocomotion();
            TurnBody();
        }

        private void UpdateLocomotion()
        {
            if (animator == null)
            {
                return;
            }

            float blend = Mathf.Clamp01(Mathf.Abs(horizontalInput));
            animator.SetFloat(BlendHash, blend, locomotionBlendTime, Time.deltaTime);
        }

        private void TurnBody()
        {
            if (body == null)
            {
                return;
            }

            float targetYaw = 0f;
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                Vector3 move = transform.right * Mathf.Sign(horizontalInput);
                targetYaw = Vector3.SignedAngle(transform.forward, move, Vector3.up);
            }

            bodyYaw = Mathf.SmoothDampAngle(bodyYaw, targetYaw, ref yawVelocity, turnSmoothTime);
            body.localRotation = Quaternion.Euler(0f, bodyYaw, 0f);
        }

        private void FixedUpdate()
        {
            Move(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            Vector3 position = transform.position;
            position.x = Mathf.Clamp(
                position.x + horizontalInput * moveSpeed * deltaTime,
                -horizontalLimit,
                horizontalLimit);

            transform.position = position;
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            locomotionBlendTime = Mathf.Max(0f, locomotionBlendTime);
            turnSmoothTime = Mathf.Max(0f, turnSmoothTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 up = Vector3.up * 2f;
            Vector3 left = new Vector3(-horizontalLimit, transform.position.y, transform.position.z);
            Vector3 right = new Vector3(horizontalLimit, transform.position.y, transform.position.z);
            Gizmos.DrawLine(left - up, left + up);
            Gizmos.DrawLine(right - up, right + up);
            Gizmos.DrawLine(left, right);
        }
    }
}
