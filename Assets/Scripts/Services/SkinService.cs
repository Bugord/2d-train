using UnityEngine;

namespace Assets.Scripts.Services
{
    public class SkinService
    {
        private SpriteRenderer _trainRenderer;

        public int RandomSkinCost;

        private GameDataService _gameDataService;

        public SkinService(int randomSkinCost = 100)
        {
            RandomSkinCost = randomSkinCost;
            _gameDataService = ServiceLocator.GetService<GameDataService>();
        }

        public void SetSkin(Sprite skin)
        {
            _gameDataService.CurrentSkinId = skin.name;
            _trainRenderer.sprite = skin;
        }

        public void UpdateSkin(SpriteRenderer trainRenderer)
        {
            _trainRenderer = trainRenderer;
            _trainRenderer.sprite = LoadSkinFromResource(_gameDataService.CurrentSkinId);
        }

        private Sprite LoadSkinFromResource(string skinId)
        {
            return Resources.Load<Sprite>($"Skins/{skinId}");
        }
    }
}
