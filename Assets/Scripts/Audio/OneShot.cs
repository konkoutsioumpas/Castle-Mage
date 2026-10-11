using UnityEngine;

namespace CastleArcher
{
    /// <summary>
    /// Plays a clip on a living object. PlayOneShot on a projectile would be cut off when that shot is destroyed.
    /// </summary>
    public static class OneShot
    {
        public static void Play(Component host, AudioClip clip, float volume)
        {
            if (host == null || clip == null)
            {
                return;
            }

            AudioSource source = host.GetComponent<AudioSource>();
            if (source == null)
            {
                source = host.gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
            }

            source.PlayOneShot(clip, volume);
        }
    }
}
