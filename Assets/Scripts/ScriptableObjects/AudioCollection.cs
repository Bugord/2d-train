using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Audio Collection", menuName = "Audio Collection", order = 51)]
    public class AudioCollection : ScriptableObject
    {
        [SerializeField] private List<AudioService.TypedAudioClip> _typedAudioClipsList;

        public AudioClip GetAudioClip(AudioClipType audioClipType)
        {
            return _typedAudioClipsList.SingleOrDefault(obj => obj.clipType == audioClipType).clip;
        }
    }
}
