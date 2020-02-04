using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class LevelService
    {
        public int Level { get; private set; }
        public int StopsCount { get; private set; }
        public float BoostedSpeed => _levelSettings.BoostedSpeed;
        private float _speed;
        private float _maxSpeed;
        private float _step;
        private LevelSettings _levelSettings;
        private GameDataService _gameDataService;
        private UIService _uiService;

        public LevelService(LevelSettings levelSettings)
        {
            _gameDataService = ServiceLocator.GetService<GameDataService>();
            _uiService = ServiceLocator.GetService<UIService>();
            _levelSettings = levelSettings;
            MapGenerator.LevelUp += OnLevelUp;
            UpdateService();
        }
        
        public float GetSpeed()
        {
            if (_speed < _maxSpeed && _uiService.IsInGame)
            {
                _speed += (_maxSpeed - _levelSettings.DefaultSpeed) * Mathf.Atan(Mathf.Lerp(0, Mathf.PI * 0.5f, _step));
                _step += _levelSettings.SpeedStep;
            }

            return _speed;
        }

        public void UpdateService()
        {
            ResetSpeed();
            Level = 0;
            StopsCount = 0;
            _maxSpeed = 6;
            UpdateMaxSpeed();
        }

        public void ResetSpeed()
        {
            _speed = _levelSettings.DefaultSpeed;
            _step = 0;
        }

        private void OnLevelUp()
        {
            Level++;

            UpdateMaxSpeed();

            if (Level == 1)
            {
                StopsCount = 1;
            }

            if (Level % 5 == 0 && StopsCount < 5)
            {
                StopsCount++;
            }
        }

        private void UpdateMaxSpeed()
        {
            _maxSpeed = Mathf.Atan(Level / 12f + 0.63f) * 5.3f + 3 + GetPlayerSkillFactor();
        }

        private float GetPlayerSkillFactor()
        {
            return Mathf.Pow(th(Level * 0.1f), 2) * (_gameDataService.AverageLevel + CloudVariables.ImportantValues[0]/(Level+1) + Level) * Mathf.Exp(-Level * 0.05f) * 0.02f;
        }

        private float th(float x)
        {
            return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
        }
    }
}