using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameData
{
    //In game coins;
    public static int Coins;
    //In game score;
    public static int Score;

    public static bool Revived {
        get
        {
            if(!PlayerPrefs.HasKey("Revived"))
                PlayerPrefs.SetInt("Revived", 0);

            return PlayerPrefs.GetInt("Revived") == 1;
        }
        set => PlayerPrefs.SetInt("Revived", value ? 1 : 0);
    }

    public static bool BonusReceived
    {
        get
        {
            if (!PlayerPrefs.HasKey("BonusReceived"))
                PlayerPrefs.SetInt("BonusReceived", 0);

            return PlayerPrefs.GetInt("BonusReceived") == 1;
        }
        set => PlayerPrefs.SetInt("BonusReceived", value ? 1 : 0);
    }


    public static int AverageLevel
    {
        get
        {
            List<int> results = new List<int>();
            results.Add(PlayerPrefs.HasKey("LastLevel1") ? PlayerPrefs.GetInt("LastLevel1") : 0);
            results.Add(PlayerPrefs.HasKey("LastLevel2") ? PlayerPrefs.GetInt("LastLevel2") : 0);
            results.Add(PlayerPrefs.HasKey("LastLevel3") ? PlayerPrefs.GetInt("LastLevel3") : 0);
            results.Add(PlayerPrefs.HasKey("LastLevel4") ? PlayerPrefs.GetInt("LastLevel4") : 0);
            results.Add(PlayerPrefs.HasKey("LastLevel5") ? PlayerPrefs.GetInt("LastLevel5") : 0);

            return (int)results.Average();
        }
    }

    public static void SetLastLevel(int level)
    {
        PlayerPrefs.SetInt("LastLevel1", level);
        PlayerPrefs.SetInt("LastLevel2", PlayerPrefs.HasKey("LastLevel1") ? PlayerPrefs.GetInt("LastLevel1") : 0);
        PlayerPrefs.SetInt("LastLevel3", PlayerPrefs.HasKey("LastLevel2") ? PlayerPrefs.GetInt("LastLevel2") : 0);
        PlayerPrefs.SetInt("LastLevel4", PlayerPrefs.HasKey("LastLevel3") ? PlayerPrefs.GetInt("LastLevel3") : 0);
        PlayerPrefs.SetInt("LastLevel5", PlayerPrefs.HasKey("LastLevel4") ? PlayerPrefs.GetInt("LastLevel4") : 0);
        PlayerPrefs.Save();
    }

    public static void ResetGame()
    {
        Coins = 0;
        Score = 0;
        Revived = false;
        BonusReceived = false;
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        UIManager.Instance.UpdateUI();
    }
}