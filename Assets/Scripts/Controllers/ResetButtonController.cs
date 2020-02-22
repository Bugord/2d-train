using System;
using Assets.Scripts.Services;
using GooglePlayGames.BasicApi.Multiplayer;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class ResetButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private PlayGamesService _playGamesService;

        private UIService _uiService;
        // Start is called before the first frame update
        void Awake()
        {
            _button.onClick.AddListener(ResetData);
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _uiService = ServiceLocator.GetService<UIService>();
            
            if (!PlayerPrefs.HasKey("NeedReset1"))
            {
                ResetData();
                PlayerPrefs.SetString("NeedReset1", String.Empty);
            }
            Destroy(gameObject);
        }

        private void ResetData()
        {
            CloudVariables.ImportantValues = new long[5];
            _playGamesService.SaveData();
            PlayerPrefs.DeleteAll();
            _uiService.UpdateMainMenu();
            _uiService.ShowStoreUI();
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
            AndroidNotificationCenter.CancelAllScheduledNotifications();
        }
    }
}
