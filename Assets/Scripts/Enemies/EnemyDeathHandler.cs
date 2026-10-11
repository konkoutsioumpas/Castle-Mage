using System.Collections;
using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Removes an enemy when its health reaches zero, after its death clip.
    /// </summary>
    /// <remarks>
    /// Subscribes for exactly as long as this component is active. Unsubscribing in
    /// OnDisable keeps a destroyed enemy from staying referenced by the health event.
    /// </remarks>
    [RequireComponent(typeof(Health))]
    public class EnemyDeathHandler : MonoBehaviour
    {
        [Tooltip("Spawned at the enemy's position on death. Left empty until a one-shot effect exists.")]
        [SerializeField] private GameObject deathEffect;

        [SerializeField] private float effectLifetime = 1.5f;

        [Tooltip("Walks by default. Death is a trigger on this controller. Left empty, the model keeps whatever controller it already has.")]
        [SerializeField] private RuntimeAnimatorController animatorController;

        [SerializeField] private string deathTrigger = "Death";

        [Tooltip("Used when the model has no Death state, such as the spitter.")]
        [SerializeField] private float fallbackDeathDelay = 0.6f;
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private float deathVolume = 0.65f;

        private static readonly int DeathHash = Animator.StringToHash("Death");

        private Health health;
        private Animator animator;
        private int scoreValue;
        private ScoreTracker scoreTracker;
        private bool dying;

        private void Awake()
        {
            health = GetComponent<Health>();
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                SkinnedMeshRenderer skin = GetComponentInChildren<SkinnedMeshRenderer>();
                if (skin != null)
                {
                    Transform rig = skin.transform;
                    while (rig.parent != null && rig.parent != transform)
                    {
                        rig = rig.parent;
                    }

                    animator = rig.gameObject.AddComponent<Animator>();
                }
            }

            if (animator == null)
            {
                return;
            }

            animator.applyRootMotion = false;
            if (animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
        }

        private void OnEnable()
        {
            health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            health.OnDied -= HandleDied;
        }

        /// <summary>Copied from the enemy definition when the spawner creates this enemy.</summary>
        public void SetScoreValue(int value)
        {
            scoreValue = Mathf.Max(0, value);
        }

        private void HandleDied()
        {
            if (dying)
            {
                return;
            }

            dying = true;
            AwardScore();
            DisableCombat();
            OneShot.Play(this, deathClip, deathVolume);

            if (deathEffect != null)
            {
                GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(effect, effectLifetime);
            }

            StartCoroutine(FinishDeath());
        }

        private void DisableCombat()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            EnemyMovement movement = GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.HoldPosition();
            }

            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null)
            {
                attack.enabled = false;
            }

            RangedEnemyAttack ranged = GetComponent<RangedEnemyAttack>();
            if (ranged != null)
            {
                ranged.enabled = false;
            }
        }

        private IEnumerator FinishDeath()
        {
            if (animator != null && HasDeathTrigger())
            {
                animator.SetTrigger(DeathHash);
                float safety = 8f;
                while (safety > 0f)
                {
                    safety -= Time.deltaTime;
                    if (!animator.IsInTransition(0))
                    {
                        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                        if (IsDeath(info) && info.normalizedTime >= 1f)
                        {
                            break;
                        }
                    }

                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(fallbackDeathDelay);
            }

            Destroy(gameObject);
        }

        private static bool IsDeath(AnimatorStateInfo info)
        {
            return info.IsName("Death") || info.IsName("Base Layer.Death");
        }

        private bool HasDeathTrigger()
        {
            if (string.IsNullOrEmpty(deathTrigger))
            {
                return false;
            }

            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Trigger && parameters[i].name == deathTrigger)
                {
                    return true;
                }
            }

            return false;
        }

        private void AwardScore()
        {
            if (scoreValue <= 0)
            {
                return;
            }

            if (scoreTracker == null)
            {
                scoreTracker = FindAnyObjectByType<ScoreTracker>();
            }

            if (scoreTracker != null)
            {
                scoreTracker.Add(scoreValue);
            }
        }
    }
}
