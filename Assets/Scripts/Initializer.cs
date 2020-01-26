using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register(new PlayGamesService());
        ServiceLocator.Register(new AchievementsService());
        ServiceLocator.Register(new LeaderBoardsService());
        ServiceLocator.Register(new IAPService());
    }
}
