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

        public void Play(AudioClip clip, float pitchLevel)
        {
            _audioSource.clip = clip;
            _audioSource.pitch = pitchLevel;
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
