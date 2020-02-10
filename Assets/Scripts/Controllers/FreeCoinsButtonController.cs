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
        [SerializeField] private string _playerPrefKey;
        
        private PlayGamesService _playGamesService;
        private UIService _uiService;

        private DateTime startSystemTime {
            get
            {
                if (!PlayerPrefs.HasKey(_playerPrefKey))
                {
                    return DateTime.MinValue;
                }
                
                return DateTime.Parse(PlayerPrefs.GetString(_playerPrefKey));
            }
            set
            {
                PlayerPrefs.SetString(_playerPrefKey, value.ToString());
            }
        }
        
        void Awake()
        {
            _button = GetComponent<Button>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _uiService = ServiceLocator.GetService<UIService>();
            
            _button.onClick.AddListener(GetFreeCoins);
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

        private void SetTimer(Action<float> callBack)
        {
            startSystemTime = DateTime.Now;
            StartTimer(true, callBack);
        }
        
        private void UpdateTimer(Action<float> callBack)
        {
            var watch = (float)(DateTime.Now - startSystemTime).TotalSeconds;
            StartTimer(true, callBack, watch);
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
