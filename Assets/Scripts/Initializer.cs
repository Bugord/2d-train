﻿using System.Collections.Generic;
using Assets.Scripts.ScriptableObjects;
using Assets.Scripts.Services;
using Services;
using UnityEngine;

namespace Assets.Scripts
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private PoolService.PoolPart[] _pools;
        [SerializeField] private AudioCollection _audioCollection;
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private AdsConfig _adsConfig;

        void OnValidate()
        {
            for (int i = 0; i < _pools.Length; i++)
            {
                _pools[i].name = _pools[i].prefab.name;
            }
        }

        private void Awake()
        {
            ServiceLocator.Register(new LeaderBoardsService());
            ServiceLocator.Register(new UIService());
            ServiceLocator.Register(new PlayGamesService());
            ServiceLocator.Register(new GameDataService());
            ServiceLocator.Register(new AchievementsService());
            ServiceLocator.Register(new LevelService(_levelSettings));
            ServiceLocator.Register(new PoolService(_pools));
            ServiceLocator.Register(new AudioService(_audioCollection));
            ServiceLocator.Register(new AdsService(_adsConfig));
            ServiceLocator.Register(new SkinService());
            ServiceLocator.Register(new NotificationService());
        }
    }
}
