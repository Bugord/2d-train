using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
    
        [SerializeField] private AudioSource _swipe;
        [SerializeField] private AudioSource _stopHit;
        [SerializeField] private AudioSource _coin;
        [SerializeField] private AudioSource _newTrain;
        [SerializeField] private AudioSource _boostStart;
        [SerializeField] private AudioSource _boostEnd;

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }

        public void Play(AudioClipType clipType)
        {
            switch (clipType)
            {
                case AudioClipType.Swipe:
                    _swipe.Play();
                    break;
                case AudioClipType.StopHit:
                    _stopHit.Play();
                    break;
                case AudioClipType.Coin:
                    _coin.Play();
                    break;
                case AudioClipType.NewTrain:
                    _newTrain.Play();
                    break;
                case AudioClipType.BoostStart:
                    _boostStart.Play();
                    break;
                case AudioClipType.BoostEnd:
                    _boostEnd.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clipType), clipType, null);
            }
        }
    }
}
