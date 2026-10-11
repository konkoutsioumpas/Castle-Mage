using UnityEngine;

namespace CastleArcher
{
    public enum PowerupType
    {
        RapidFire,
        MultiShot,
        CastleRepair
    }

    /// <summary>
    /// The stats for one power-up. The collector switches on <see cref="Type"/> rather than
    /// a subclass per effect: three cases are easier to read than three asset types.
    /// </summary>
    [CreateAssetMenu(fileName = "Powerup", menuName = "Castle Archer/Power-up Definition")]
    public class PowerupDefinition : ScriptableObject
    {
        [SerializeField] private PowerupType type;
        [SerializeField] private string displayName = "Power-up";
        [SerializeField] private GameObject prefab;

        [Tooltip("How long Rapid Fire and Multi-Shot last. Castle Repair ignores this.")]
        [SerializeField] private float duration = 8f;

        [SerializeField] private Color color = Color.white;
        [SerializeField] private Sprite icon;
        [SerializeField] private AudioClip pickupSound;

        public PowerupType Type => type;
        public string DisplayName => displayName;
        public GameObject Prefab => prefab;
        public float Duration => duration;
        public Color Color => color;
        public Sprite Icon => icon;
        public AudioClip PickupSound => pickupSound;

        private void OnValidate()
        {
            duration = Mathf.Max(0f, duration);
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = type.ToString();
            }
        }
    }
}
