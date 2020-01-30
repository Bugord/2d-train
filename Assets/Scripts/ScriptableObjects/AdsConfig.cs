using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Ads Config", menuName = "Ads Config", order = 53)]
    public class AdsConfig : ScriptableObject
    {
        [SerializeField] private string _gameId;
        [SerializeField] private string _gameOverPlacementId;
        [SerializeField] private string _reviveVideoPlacementId;
        [SerializeField] private string _bonusVideoPlacementId;
        [SerializeField] private bool _testMode;

        public string GameId => _gameId;
        public string GameOverPlacementId => _gameOverPlacementId;
        public string ReviveVideoPlacementId => _reviveVideoPlacementId;
        public string BonusVideoPlacementId => _bonusVideoPlacementId;
        public bool TestMode => _testMode;
    }
}
