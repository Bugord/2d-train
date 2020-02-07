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
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _runningColor;
        [SerializeField] private int _freeCoinsCount;
        [SerializeField] private GameObject _lightObject;

        private AdsService _adsService;
        private PlayGamesService _playGamesService;
        private UIService _uiService;

        void Awake()
        {
            _button = GetComponent<Button>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _uiService = ServiceLocator.GetService<UIService>();
            
            _button.onClick.AddListener(_adsService.ShowFreeCoinsVideoAdvertisement);
            _adsService.FreeCoins += GetFreeCoins;
            TimerEnded += OnTimerEnded;
        }

        private void OnEnable()
        {
            _button.interactable = false;
            _text.color = _runningColor;
            _lightObject.SetActive(false);
            UpdateTimer(CallBack);
        }

        private void OnTimerEnded()
        {
            _text.text = "FREE COINS";
            _text.color = _readyColor;
            _button.interactable = true;
            _lightObject.SetActive(true);
        }

        private void GetFreeCoins()
        {
            CloudVariables.ImportantValues[1] += _freeCoinsCount;
            _playGamesService.SaveData();
            _uiService.UpdateMainMenu();
            SetTimer(CallBack);
            _button.interactable = false;
            _text.color = _runningColor;
            _lightObject.SetActive(false);
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
