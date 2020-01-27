using Assets.Scripts.Services;
using UnityEngine;

namespace Assets.Scripts
{
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
}
