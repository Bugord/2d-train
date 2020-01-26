using GooglePlayGames;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class AchievementsService
    {
        public void UnlockAchievement(string id)
        {
            Social.ReportProgress(id, 100, success => { });
        }

        public void IncrementAchievement(string id, int stepsToIncrement)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { });
        }

        public void ShowAchievementsUI()
        {
            Social.ShowAchievementsUI();
        }
    }
}
