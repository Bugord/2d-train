using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameDataFields
{
    BestScore,
    Coins
}

public static class GameData
{
    private static int LastScore;
    private static int BestScore;
    public static int InGameCoins;
    
    public static void UpdateBestScore()
    {
        if (LastScore > BestScore)
        {
            BestScore = LastScore;
            PlayerPrefs.SetInt(GameDataFields.BestScore.ToString(), BestScore);
        }

        LastScore = 0;
        PlayerPrefs.Save();
    }

    public static void SetLastScore(int score)
    {
        LastScore = score;
        UIManager.Instance.SetScore(score);
    }

    public static void SetInGameCoins()
    {
        InGameCoins++;
        UIManager.Instance.SetCoins(InGameCoins);
    }

    public static void AddCoins()
    {
        var currentCoins = PlayerPrefs.GetInt(GameDataFields.Coins.ToString());
        PlayerPrefs.SetInt(GameDataFields.Coins.ToString(), currentCoins + InGameCoins);
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
