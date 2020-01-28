using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Enums;
using Assets.Scripts.ScriptableObjects;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class AudioService
    {
        private PoolService _poolService;
        private AudioCollection _audioCollection;

        [Serializable]
        public struct TypedAudioClip
        {
            public AudioClip clip;
            public AudioClipType clipType;
        }

        public AudioService(AudioCollection audioCollection)
        {
            _poolService = ServiceLocator.GetService<PoolService>();
            _audioCollection = audioCollection;
        }

        public void Play(AudioClipType clipType, float pitchLevel = 1)
        {
            var audioSourceController = _poolService.GetObject<AudioSourceController>("AudioSource");
            audioSourceController.Play(_audioCollection.GetAudioClip(clipType), pitchLevel);
        }
    }
}
