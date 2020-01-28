using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Level Settings", menuName = "Level Settings", order = 52)]
    public class LevelSettings : ScriptableObject
    {
        [SerializeField] private float _boostedSpeed;
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private float _speedStep;

        public float BoostedSpeed => _boostedSpeed;
        public float DefaultSpeed => _defaultSpeed;
        public float SpeedStep => _speedStep;
    }
}
