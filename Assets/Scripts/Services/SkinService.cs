using System;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class SkinService
    {
        private SpriteRenderer _trainRenderer;

        public int RandomSkinCost;

        public event Action<Sprite> UpdateSelectedTrainPreview; 

        private GameDataService _gameDataService;

        public SkinService(int randomSkinCost = 300)
        {
            RandomSkinCost = randomSkinCost;
            _gameDataService = ServiceLocator.GetService<GameDataService>();
        }

        public void SetSkin(Sprite skin)
        {
            _gameDataService.CurrentSkinId = skin.name;
            _trainRenderer.sprite = skin;
            UpdateSelectedTrainPreview?.Invoke(skin);
        }

        public void UpdateSkin(SpriteRenderer trainRenderer)
        {
            _trainRenderer = trainRenderer;
            var sprite = LoadSkinFromResource(_gameDataService.CurrentSkinId);
            _trainRenderer.sprite = sprite;
        }

        public Sprite GetCurrentSkin()
        {
            return _trainRenderer.sprite;
        }

        private Sprite LoadSkinFromResource(string skinId)
        {
            return Resources.Load<Sprite>($"Skins/{skinId}");
        }
    }
}
