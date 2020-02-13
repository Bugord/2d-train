using System;
using UI;

namespace Assets.Scripts.Services
{
    public class UIService
    {
        public bool IsInGame;
        public bool IsInTutorial = false;
        public PanelBase CurrentPanel;
        
        public event Action<int> InGameDistanceUpdate;
        public event Action<int> InGameCoinsUpdate;
        public event Action GameRestart;
        public event Action OpenMainMenu;
        public event Action UpdateMainMenuData;
        public event Action<bool> SetActiveInGameUI;
        public event Action OpenEndGameMenu;
        public event Action OpenReviveMenu; 
        public event Action<bool> SetActiveStoreMenu;
        public event Action OpenPauseMenu;
        public event Action EndGameBackButton;

        public UIService()
        {
            InputManager.BackButton += OnBackButton;
        }

        public void UpdateInGameDistance(int distance)
        {
            InGameDistanceUpdate?.Invoke(distance);
        }

        public void UpdateInGameCoins(int coins)
        {
            InGameCoinsUpdate?.Invoke(coins);
        }

        public void ShowReviveMenu(bool canRevive = false)
        {
            if (canRevive)
            {
                SetActiveInGameUI?.Invoke(false);
                OpenReviveMenu?.Invoke();
            }
            else
            {
                ShowEndGameMenu();
            }
        }

        public void ShowEndGameMenu()
        {
            SetActiveInGameUI?.Invoke(false);
            OpenEndGameMenu?.Invoke();
        }
        
        public void SetPause()
        {
            IsInGame = false;
            OpenPauseMenu?.Invoke();
            SetActiveInGameUI?.Invoke(false);
        }

        public void ExitToMainMenu()
        {
            IsInGame = false;
            GameRestart?.Invoke();
            SetActiveInGameUI?.Invoke(false);
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            OpenMainMenu?.Invoke();
            SetActiveStoreMenu?.Invoke(false);
        }

        public void UpdateMainMenu()
        {
            UpdateMainMenuData?.Invoke();
        }

        public void ShowInGameUI()
        {
            IsInGame = true;
            SetActiveInGameUI?.Invoke(true);
        }

        public void ShowStoreUI()
        {
            SetActiveStoreMenu?.Invoke(true);
        }

        private void OnBackButton()
        {
            switch (CurrentPanel)
            {
                case PausePanelController _:
                    ShowInGameUI();
                    break;
                case InGameUIController _:
                    SetPause();
                    break;
                case StoreController _:
                    ShowMainMenu();
                    break;
                case EndGameMenuController _:
                    EndGameBackButton?.Invoke();
                    break;
                case ReviveMenuController _:
                    ShowEndGameMenu();
                    break;
            }
        }
    }
}
