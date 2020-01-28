using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private PoolService.PoolPart[] _pools;
        [SerializeField] private AudioCollection _audioCollection;

        void OnValidate()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                _pools[i].name = _pools[i].prefab.name;
            }
        }

        private void Awake()
        {
            ServiceLocator.Register(new PlayGamesService());
            ServiceLocator.Register(new AchievementsService());
            ServiceLocator.Register(new LeaderBoardsService());
            ServiceLocator.Register(new IAPService());
            ServiceLocator.Register(new PoolService(_pools));
            ServiceLocator.Register(new AudioService(_audioCollection));
        }
    }
}
