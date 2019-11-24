using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal enum GameDataFields
{
    BestScore,
    Coins, 
    Revived,
    BonusReceived
}

public static class GameData
{
    public static int LastScore;

    public static int BestScore => PlayerPrefs.HasKey(GameDataFields.BestScore.ToString())
        ? PlayerPrefs.GetInt(GameDataFields.BestScore.ToString())
        : 0;
    public static int InGameCoins;

    public static int LastLevel
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

    public static int Revived => PlayerPrefs.HasKey(GameDataFields.Revived.ToString())
        ? PlayerPrefs.GetInt(GameDataFields.Revived.ToString())
        : 0;

    public static void SetRevived(int x)
    {
        PlayerPrefs.SetInt(GameDataFields.Revived.ToString(), x);
    }
    
    public static int BonusReceived => PlayerPrefs.HasKey(GameDataFields.BonusReceived.ToString())
        ? PlayerPrefs.GetInt(GameDataFields.BonusReceived.ToString())
        : 0;

    public static void SetBonusReceived(int x)
    {
        PlayerPrefs.SetInt(GameDataFields.BonusReceived.ToString(), x);
    }

    public static void ResetBonusAndRevive()
    {
        SetRevived(0);
        SetBonusReceived(0);
    }

    public static void UpdateBestScore()
    {
        if (LastScore > BestScore)
        {
            PlayerPrefs.SetInt(GameDataFields.BestScore.ToString(), LastScore);
        }

        LastScore = 0;
        PlayerPrefs.Save();
    }

    public static void SetLastScore(int score)
    {
        LastScore = score;
        UIManager.Instance.SetScore(score);
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

    public static void SetInGameCoins()
    {
        InGameCoins++;
        UIManager.Instance.SetCoins(InGameCoins);
    }

    public static void AddBonusToInGameCoins()
    {
        InGameCoins += InGameCoins;
        UIManager.Instance.SetCoins(InGameCoins);
    }

    public static void AddCoins()
    {
        var currentCoins = PlayerPrefs.GetInt(GameDataFields.Coins.ToString());
        PlayerPrefs.SetInt(GameDataFields.Coins.ToString(), currentCoins + InGameCoins);
        PlayerPrefs.Save();
    }

    public static void AddCoins(int coins)
    {
        var currentCoins = PlayerPrefs.GetInt(GameDataFields.Coins.ToString());
        PlayerPrefs.SetInt(GameDataFields.Coins.ToString(), currentCoins + coins);
        PlayerPrefs.Save();
    }

    public static void RemoveCoins(int coins)
    {
        var currentCoins = PlayerPrefs.GetInt(GameDataFields.Coins.ToString());
        PlayerPrefs.SetInt(GameDataFields.Coins.ToString(), currentCoins - coins);
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        UIManager.Instance.UpdateUI();
    }
}