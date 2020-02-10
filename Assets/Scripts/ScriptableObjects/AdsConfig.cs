using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Ads Config", menuName = "Ads Config", order = 53)]
    public class AdsConfig : ScriptableObject
    {
        [SerializeField] private string _gameId;
        [SerializeField] private string _gameOverPlacementId;
        [SerializeField] private float _gameOverPlacementTimeout;
        [SerializeField] private string _reviveVideoPlacementId;
        [SerializeField] private string _bonusVideoPlacementId;
        [SerializeField] private string _swipyRailsBannerPlacement;
        [SerializeField] private string _freeCoinsVideoPlacementId;
        [SerializeField] private bool _testMode;

        public string GameId => _gameId;
        public string GameOverPlacementId => _gameOverPlacementId;
        public float GameOverPlacementTimeout => _gameOverPlacementTimeout;
        public string ReviveVideoPlacementId => _reviveVideoPlacementId;
        public string BonusVideoPlacementId => _bonusVideoPlacementId;
        public string BannerPlacementId => _swipyRailsBannerPlacement;
        public string FreeCoinsPlacementId => _freeCoinsVideoPlacementId;
        public bool TestMode => _testMode;
    }
}
