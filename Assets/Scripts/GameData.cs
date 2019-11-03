using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static int LastScore;
    public static int BestScore;

    public static void UpdateBestScore()
    {
        if (LastScore > BestScore)
        {
            BestScore = LastScore;
        }

        LastScore = 0;
    }
}
