using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ReviveMenuController : PanelBase
    {
        [SerializeField] private Button _reviveButton;
        [SerializeField] private TimerButtonController _timerButton;
        [SerializeField] private Button _noThanksButton;

        private AdsService _adsService;
        private UIService _uiService;
        // Start is called before the first frame update
        void Awake()
        {
            _adsService = ServiceLocator.GetService<AdsService>();
            _uiService = ServiceLocator.GetService<UIService>();

            _uiService.OpenReviveMenu += Open;
            _reviveButton.onClick.AddListener(_adsService.ShowReviveVideoAdvertisement);
            _timerButton.TimerEnded += ShowEndGameMenu;
            _noThanksButton.onClick.AddListener(ShowEndGameMenu);
            
            _adsService.ReviveAdvertisementUpdate += delegate (bool isReady) { _reviveButton.interactable = isReady; };
            _adsService.TrainRevive += ShowPauseMenu;
        }

        private void Open()
        {
            SetActivePanel(true);
            _timerButton.StartTimer(false, null);
        }
        
        private void ShowEndGameMenu()
        {
            SetActivePanel(false);
            _uiService.ShowEndGameMenu();
        }

        private void ShowPauseMenu()
        {
            SetActivePanel(false);
            _uiService.SetPause();
        }
    }
}
