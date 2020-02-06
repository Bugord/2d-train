using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class GameDataService
    {
        //In game coins;
        public int Coins;
        //In game score;
        public int Score;

        public bool Revived {
            get
            {
                if(!PlayerPrefs.HasKey("Revived"))
                    PlayerPrefs.SetInt("Revived", 0);

                return PlayerPrefs.GetInt("Revived") == 1;
            }
            set => PlayerPrefs.SetInt("Revived", value ? 1 : 0);
        }

        public bool BonusReceived
        {
            get
            {
                if (!PlayerPrefs.HasKey("BonusReceived"))
                    PlayerPrefs.SetInt("BonusReceived", 0);

                return PlayerPrefs.GetInt("BonusReceived") == 1;
            }
            set => PlayerPrefs.SetInt("BonusReceived", value ? 1 : 0);
        }

        public bool SoundOn
        {
            get
            {
                if (!PlayerPrefs.HasKey("SoundOn"))
                    PlayerPrefs.SetInt("SoundOn", 1);

                return PlayerPrefs.GetInt("SoundOn") == 1;
            }
            set => PlayerPrefs.SetInt("SoundOn", value ? 1 : 0);
        }

        public string CurrentSkinId
        {
            get
            {
                if (!PlayerPrefs.HasKey("CurrentSkin"))
                    PlayerPrefs.SetString("CurrentSkin", "Train_0");

                return PlayerPrefs.GetString("CurrentSkin");
            }
            set => PlayerPrefs.SetString("CurrentSkin", value);
        }

        public int AverageLevel
        {
            get
            {
                List<int> results = new List<int>
                {
                    PlayerPrefs.HasKey("LastLevel1") ? PlayerPrefs.GetInt("LastLevel1") : 0,
                    PlayerPrefs.HasKey("LastLevel2") ? PlayerPrefs.GetInt("LastLevel2") : 0,
                    PlayerPrefs.HasKey("LastLevel3") ? PlayerPrefs.GetInt("LastLevel3") : 0,
                    PlayerPrefs.HasKey("LastLevel4") ? PlayerPrefs.GetInt("LastLevel4") : 0,
                    PlayerPrefs.HasKey("LastLevel5") ? PlayerPrefs.GetInt("LastLevel5") : 0
                };

                return (int)results.Average();
            }
        }

        public void SetLastLevel(int level)
        {
            PlayerPrefs.SetInt("LastLevel1", level);
            PlayerPrefs.SetInt("LastLevel2", PlayerPrefs.HasKey("LastLevel1") ? PlayerPrefs.GetInt("LastLevel1") : 0);
            PlayerPrefs.SetInt("LastLevel3", PlayerPrefs.HasKey("LastLevel2") ? PlayerPrefs.GetInt("LastLevel2") : 0);
            PlayerPrefs.SetInt("LastLevel4", PlayerPrefs.HasKey("LastLevel3") ? PlayerPrefs.GetInt("LastLevel3") : 0);
            PlayerPrefs.SetInt("LastLevel5", PlayerPrefs.HasKey("LastLevel4") ? PlayerPrefs.GetInt("LastLevel4") : 0);
            PlayerPrefs.Save();
        }

        public void ResetGame()
        {
            Coins = 0;
            Score = 0;
            Revived = false;
            BonusReceived = false;
        }

        public void UpdateCloudVariables()
        {
            if (Score > CloudVariables.ImportantValues[0])
            {
                CloudVariables.ImportantValues[0] = Score;
            }

            CloudVariables.ImportantValues[1] += Coins;
            CloudVariables.ImportantValues[4] = Score;
        }
    }
}