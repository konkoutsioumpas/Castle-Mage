using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Slow yaw, a small pitch wobble, and a bob. The repair icon uses this in place of a
    /// particle glow. It moves its own transform, so the pickup root still slides on its own.
    /// </summary>
    public class HoverSpin : MonoBehaviour
    {
        [SerializeField] private float yawDegreesPerSecond = 90f;

        [Tooltip("How far above the placed position the icon rises. The placed position is the bottom of the bob.")]
        [SerializeField] private float bobHeight = 0.2f;

        [SerializeField] private float bobFrequency = 2f;

        [Tooltip("Pitch either side of upright, in degrees.")]
        [SerializeField] private float pitchDegrees = 15f;

        [SerializeField] private float pitchFrequency = 1.5f;

        private Vector3 restPosition;

        private void Awake()
        {
            restPosition = transform.localPosition;
        }

        private void Update()
        {
            float time = Time.time;
            float yaw = time * yawDegreesPerSecond;
            float pitch = Mathf.Sin(time * pitchFrequency) * pitchDegrees;
            float bob = (Mathf.Sin(time * bobFrequency) + 1f) * 0.5f * bobHeight;

            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.localPosition = restPosition + Vector3.up * bob;
        }

        private void OnValidate()
        {
            bobHeight = Mathf.Max(0f, bobHeight);
            bobFrequency = Mathf.Max(0f, bobFrequency);
            pitchDegrees = Mathf.Max(0f, pitchDegrees);
            pitchFrequency = Mathf.Max(0f, pitchFrequency);
        }
    }
}
