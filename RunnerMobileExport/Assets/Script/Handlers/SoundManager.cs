using System.Linq;
using UnityEngine;

namespace Scripts.SoundEffects
{
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField] private float defaultVolume;
        [SerializeField] private bool pauseable;
        [SerializeField, Range(2, 8)] private int poolSize;

        private AudioSource[] pool;

        private void Awake()
        {
            pool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.priority = 0;
                pool[i] = source;
            }

            OnVolumeChanged(defaultVolume);

            // Subscribe OnVolumeChanged and OnPauseTriggered to axternal events that handles volume change and pausing

        }

        public void Play(AudioClip clip, float pitch = 1)
        {
            var source = pool.FirstOrDefault(audioSource => !audioSource.isPlaying);
            if (source == null)
            {
                source = pool[0];
                Debug.LogWarning("Pool does not have enough audio sources to satify the requests");
            }
            source.clip = clip;
            source.pitch = pitch;
            source.Play();
        }

        private void OnVolumeChanged(float volume)
        {
            foreach (AudioSource source in pool) source.volume = volume;
        }


        private void OnPauseTriggered(bool isPaused)
        {
            if (!pauseable) return;
            foreach (AudioSource source in pool)
            {
                if (isPaused) source.Pause();
                else source.UnPause();
            }
        }

        private void OnDestroy()
        {
            // Remember to unsubscribe from events you subscribed in Awake()
            // SettingsMenu.OnSFXVolumeChanged -= OnVolumeChanged;
            // PauseTrigger.OnPauseTriggered -= OnPauseTriggered;
        }
    }
}