using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Keeps the head toward the enemies while the body yaw follows movement.
    /// </summary>
    /// <remarks>
    /// Unity only sends <see cref="OnAnimatorIK"/> to a script on the same object as the
    /// Animator, and only for a layer with IK Pass enabled. <see cref="PlayerMovement"/>
    /// adds this next to the animator and calls <see cref="Initialize"/>.
    /// </remarks>
    public class ArcherHeadLook : MonoBehaviour
    {
        [SerializeField] private float lookDistance = 12f;

        [Tooltip("Metres above the enemy root, or above the archer when nobody is targeted.")]
        [SerializeField] private float lookHeight = 1.5f;

        [Tooltip("How much of the look the chest takes. The hips stay with the run.")]
        [SerializeField] private float bodyWeight = 0.15f;

        [SerializeField] private float headWeight = 1f;
        [SerializeField] private float eyesWeight = 0.4f;

        [Tooltip("0 looks anywhere, 0.5 stops at about 90 degrees, 1 cannot look away from the animation.")]
        [SerializeField] private float clampWeight = 0.3f;

        private Animator animator;
        private Transform player;
        private ArcherTargeting targeting;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>The player root stays facing the field. The current target, when there is one, is the look point.</summary>
        public void Initialize(Transform playerRoot, ArcherTargeting aim)
        {
            player = playerRoot;
            targeting = aim;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null || player == null || layerIndex != animator.layerCount - 1)
            {
                return;
            }

            animator.SetLookAtWeight(1f, bodyWeight, headWeight, eyesWeight, clampWeight);
            animator.SetLookAtPosition(LookPoint());
        }

        private Vector3 LookPoint()
        {
            Transform target = targeting != null ? targeting.CurrentTarget : null;
            if (target != null)
            {
                return target.position + Vector3.up * lookHeight;
            }

            return player.position + player.forward * lookDistance + Vector3.up * lookHeight;
        }

        private void OnValidate()
        {
            lookDistance = Mathf.Max(0.1f, lookDistance);
            lookHeight = Mathf.Max(0f, lookHeight);
            bodyWeight = Mathf.Clamp01(bodyWeight);
            headWeight = Mathf.Clamp01(headWeight);
            eyesWeight = Mathf.Clamp01(eyesWeight);
            clampWeight = Mathf.Clamp01(clampWeight);
        }
    }
}
