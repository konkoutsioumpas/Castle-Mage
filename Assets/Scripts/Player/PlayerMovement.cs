using UnityEngine;
using UnityEngine.InputSystem;

namespace CastleArcher
{
    /// <summary>
    /// Moves the archer left and right along the rampart and keeps him inside the arena.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;

        [Tooltip("Furthest the archer's centre may travel from x = 0. Sized to the camera view, not the walls: the side towers sit near ±14.7, but at the rampart the 16:9 frame only shows about ±10.")]
        [SerializeField] private float horizontalLimit = 10f;

        [SerializeField] private InputAction moveAction;

        private float horizontalInput;

        /// <summary>Signed horizontal input, in the range -1 to 1, for animation to read later.</summary>
        public float HorizontalInput => horizontalInput;

        // The action is enabled for exactly as long as this component is active. Enabling it in
        // Start and never disabling it leaks the action whenever the scene is reloaded.
        private void OnEnable()
        {
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        private void Update()
        {
            horizontalInput = moveAction.ReadValue<Vector2>().x;
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
