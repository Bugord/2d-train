using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Services;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

namespace UI
{
    public class SkinStoreController : PanelBase
    {
        [SerializeField] private Button _getRandomSkin;
        [SerializeField] private Text _randomSkinButtonText;
        [SerializeField] private Image _selectedTrainPreview;

        [SerializeField] private HorizontalScrollSnap _scroll;
        [SerializeField] private List<RectTransform> _pages;

        private List<SkinButton> _skinsList;

        private PlayGamesService _playGamesService;
        private SkinService _skinService;
        private UIService _uiService;
        private AchievementsService _achievementsService;
        
        public event Action UpdateStoreCoins;

        private void Awake()
        {
            _uiService = ServiceLocator.GetService<UIService>();
            _playGamesService = ServiceLocator.GetService<PlayGamesService>();
            _skinService = ServiceLocator.GetService<SkinService>();
            _achievementsService = ServiceLocator.GetService<AchievementsService>();
            
            _skinsList = new List<SkinButton>();
            _getRandomSkin.onClick.AddListener(GetRandomSkin);
            _pages.ForEach(page => _skinsList.AddRange(page.GetComponentsInChildren<SkinButton>()));
            _skinService.UpdateSelectedTrainPreview += UpdateSelectedPreview;
            UpdateSkins();
            _selectedTrainPreview.sprite = _skinService.GetCurrentSkin();
        }
        
        private void GetRandomSkin()
        {
            var lockedSkins = _skinsList.Where(skin => !skin.IsUnlocked).ToList();
            var skinCost = GetSkinCost();
            if (lockedSkins.Count <= 0 || skinCost > CloudVariables.ImportantValues[1]) return;
            {
                var newSkin = lockedSkins[Random.Range(0, lockedSkins.Count - 1)].UnlockSkin().SkinImage.sprite;
                _skinService.SetSkin(newSkin);

                int newSkinPageId = (int)Math.Truncate((decimal) Convert.ToInt32(newSkin.name.Split('_')[1]) / 9);

                int deltaPage = newSkinPageId - _scroll.CurrentScreen();
                
                if (deltaPage > 0)
                {
                    _scroll.NextScreen(Math.Abs(deltaPage));
                }
                else
                {
                    _scroll.PreviousScreen(Math.Abs(deltaPage));
                }
                

                CloudVariables.ImportantValues[1] -= skinCost;

                var binString = "1";
                _skinsList.ForEach(skin =>
                {
                    if (skin.SkinImage.sprite.name == "Train_0") return;

                    if (skin.IsUnlocked)
                    {
                        binString += "1";
                    }
                    else
                    {
                        binString += "0";
                    }
                });

                CloudVariables.ImportantValues[3] = Convert.ToInt64(binString, 2);
            
                _playGamesService.SaveData(); 
                _uiService.UpdateMainMenu();
                UpdateStoreCoins?.Invoke();
                _randomSkinButtonText.text = $"{GetSkinCost()} COINS";
                
                _achievementsService.UnlockAchievement(GPGSIds.achievement_new_customer);
                _achievementsService.IncrementAchievement(GPGSIds.achievement_train_station, 1);
                _achievementsService.IncrementAchievement(GPGSIds.achievement_railway_tycoon, 1);
            }
        }

        private int GetSkinCost()
        {
            var unLockedSkinsCount = _skinsList.Where(skin => skin.IsUnlocked).ToList().Count;
            var skinCost = _skinService.RandomSkinCost + 250 * (unLockedSkinsCount - 1);
            if (skinCost > 1500)
            {
                skinCost = 1500;
            }

            return skinCost;
        }

        public void UpdateSkins()
        {
            var binString = Convert.ToString(CloudVariables.ImportantValues[3], 2);
            var array = binString.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == '1')
                {
                    _skinsList?[i].UnlockSkin();
                }
            }
            
            _randomSkinButtonText.text = $"{GetSkinCost()} COINS";
        }

        private void UpdateSelectedPreview(Sprite sprite)
        {
            _selectedTrainPreview.sprite = sprite;
        }
    }
}
