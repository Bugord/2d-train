using System;
using Assets.Scripts.Services;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class FreeCoinsButtonController : TimerButtonController
    {
        private Button _button;
        [SerializeField] private Text _text;

        private AdsService _adsService;

        void Awake()
        {
            _button = GetComponent<Button>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _button.onClick.AddListener(_adsService.ShowFreeCoinsVideoAdvertisement);
            _adsService.FreeCoins += GetFreeCoins;
            TimerEnded += OnTimerEnded;
        }

        private void OnEnable()
        {
            UpdateTimer(CallBack);
        }

        private void OnTimerEnded()
        {
            _text.text = "FREE COINS";
            _button.interactable = true;
        }

        private void GetFreeCoins()
        {
            SetTimer(CallBack);
            _button.interactable = false;
        }

        private void CallBack(float currentTime)
        {
            _text.text = GetTime((float)Math.Round(time - currentTime));
        }

        private string GetTime(float time)
        {
            TimeSpan t = TimeSpan.FromSeconds( time );
            return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
        }
    }
}
