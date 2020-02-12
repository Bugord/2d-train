using System;
using Assets.Scripts;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class StoreController : PanelBase
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Text _coins;
        [SerializeField] private SkinStoreController _skinStoreController;
        [SerializeField] private CoinsStoreController _coinsStoreController;
        [SerializeField] private Button _trainsButton;
        [SerializeField] private Button _coinsButton;

        private UIService _uiService;
        
        // Start is called before the first frame update
        void Awake()
        {
            _uiService = ServiceLocator.GetService<UIService>();
            
            _uiService.SetActiveStoreMenu += SetActive;
            _exitButton.onClick.AddListener(BackToMainMenu);
            _skinStoreController.UpdateStoreCoins += () => { UpdateCoins(true); };
            _trainsButton.onClick.AddListener(OpenSkinStore);
            _coinsButton.onClick.AddListener(OpenCoinsStore);
            OpenSkinStore();
            UpdateCoins();
            _uiService.OpenMainMenu += delegate { SetActivePanel(false); };
        }

        private void OnEnable()
        {
            OpenSkinStore();
        }

        private void OpenSkinStore()
        {
            _trainsButton.interactable = false;
            _coinsButton.interactable = true;
            _skinStoreController.SetActivePanel(true);
            _coinsStoreController.SetActivePanel(false);
        }
        
        private void OpenCoinsStore()
        {
            _coinsButton.interactable = false;
            _trainsButton.interactable = true;
            _skinStoreController.SetActivePanel(false);
            _coinsStoreController.SetActivePanel(true);
        }

        private void SetActive(bool isActive)
        {
            SetActivePanel(isActive);
            if (!isActive) return;
            
            _uiService.CurrentPanel = this;
            UpdateCoins();
        }

        private void UpdateCoins(bool isAnimated = false)
        {
            if (isAnimated)
            {
                StartCoroutine(UpdateText(_coins, int.Parse(_coins.text), CloudVariables.GetCoins()));
            }
            else
            {
                _coins.text = CloudVariables.GetCoins().ToString();
            }
        }

        private void BackToMainMenu()
        {
            SetActivePanel(false);
            _uiService.ShowMainMenu();
        }
    }
}
