using System;

namespace Assets.Scripts.Services
{
    public class UIService
    {
        public bool IsInGame;
        
        public event Action<int> InGameDistanceUpdate;
        public event Action<int> InGameCoinsUpdate;
        public event Action GameRestart;
        public event Action OpenMainMenu;
        public event Action UpdateMainMenuData;
        public event Action<bool> SetActiveInGameUI;
        public event Action<bool> OpenEndGameMenu;
        public event Action OpenPauseMenu;

        public event Action OpenStoreMenu;

        public void UpdateInGameDistance(int distance)
        {
            InGameDistanceUpdate?.Invoke(distance);
        }

        public void UpdateInGameCoins(int coins)
        {
            InGameCoinsUpdate?.Invoke(coins);
        }

        public void ShowEndGameMenu(bool canRevive = false)
        {
            SetActiveInGameUI?.Invoke(false);
            OpenEndGameMenu?.Invoke(canRevive);
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
            OpenStoreMenu?.Invoke();
        }
    }
}
