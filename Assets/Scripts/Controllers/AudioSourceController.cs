using Assets.Scripts.Enums;
using Assets.Scripts.ObjectsPool;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AudioSourceController : PoolObject
    {
        private AudioSource _audioSource;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, SwipeDirection direction, float volume)
        {
            _audioSource.clip = clip;
            if (direction == SwipeDirection.Right)
            {
                _audioSource.panStereo = 0.4f;
            }
            else if (direction == SwipeDirection.Left)
            {
                _audioSource.panStereo = -0.4f;
            }

            _audioSource.pitch = 1;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        public void Play(AudioClip clip, float pitchLevel, float volume)
        {
            _audioSource.clip = clip;
            _audioSource.pitch = pitchLevel;
            _audioSource.panStereo = 0;
            _audioSource.volume = volume;
            _audioSource.Play();
        }

        public void Update()
        {
            if (!_audioSource.isPlaying)
            {
                ReturnToPool();
            }
        }
    }
}
