using System;
using Assets.Scripts.Services;
using Services;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

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
        private NotificationService _notificationService;

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

        private DateTime startDay
        {
            get
            {
                if (!PlayerPrefs.HasKey("StartDay"))
                {
                    return DateTime.Now;
                }
                
                return DateTime.Parse(PlayerPrefs.GetString("StartDay"));
            }
            set
            {
                PlayerPrefs.SetString("StartDay", value.ToString());
            }
        }

        private int lastTime
        {
            get => !PlayerPrefs.HasKey("LastTime") ? 0 : PlayerPrefs.GetInt("LastTime");
            set => PlayerPrefs.SetInt("LastTime", value);
        }

        void Awake()
        {
            _button = GetComponent<Button>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _notificationService = ServiceLocator.GetService<NotificationService>();
            
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

        void OnApplicationFocus(bool isOnFocus)
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
            if (startDay.Day < DateTime.Now.Day)
            {
                startDay = DateTime.Now;
                lastTime = 60;
                time = 60;
            }
            else
            {
                switch (lastTime)
                {
                    case 0:
                        time = 60;
                        break;
                    case 60:
                        time = 300;
                        break;
                    case 300:
                        time = 600;
                        break;
                    case 600:
                        time = 1800;
                        break;
                    case 1800:
                        time = 3600;
                        break;
                    case 3600:
                        time = 10800;
                        break;
                    case 10800:
                        time = 21600;
                        break;
                    case 21600:
                        time = 32400;
                        break;
                }
                lastTime = (int)time;
            }
            
            _notificationService.ShowFreeCoinsNotification(time);
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
            time = lastTime;
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
