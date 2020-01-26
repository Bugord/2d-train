using UnityEngine;

namespace Assets.Scripts.Services
{
    public class LeaderBoardsService 
    {
        public void AddScoreToLeaderBoard(string leaderbourdId, long score)
        {
            Social.ReportScore(score, leaderbourdId, success => { });
        }

        public void ShowLeaderBoardUI()
        {
            Social.ShowLeaderboardUI();
        }
    }
}
