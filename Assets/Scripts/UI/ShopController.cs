﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopController : PanelBase
{
    public Button BackButton;
    [SerializeField] private Button _getRandomSkin;

    [SerializeField] private List<RectTransform> _pages;

    private List<SkinButton> _skinsList;
    
    // Start is called before the first frame update
    void Start()
    {
        _skinsList = new List<SkinButton>();
        BackButton.onClick.AddListener(GoBack);
        _getRandomSkin.onClick.AddListener(GetRandomSkin);
        _pages.ForEach(page => _skinsList.AddRange(page.GetComponentsInChildren<SkinButton>()));
    }

    private void GetRandomSkin()
    {
        var lockedSkins = _skinsList.Where(skin => !skin.IsUnlocked).ToList();
        if (lockedSkins.Count <= 0 || SkinManager.Instance.RandomSkinCost > CloudVariables.ImportantValues[1]) return;
        {
            var newSkin = lockedSkins[Random.Range(0, lockedSkins.Count - 1)].UnlockSkin().SkinImage.sprite;
            SkinManager.Instance.SetSkin(newSkin);

            CloudVariables.ImportantValues[1] -= SkinManager.Instance.RandomSkinCost;

            var binString = "1";
            _skinsList.ForEach(skin =>
            {
                if (skin.SkinImage.sprite.name == "Train_0") return;

                if (skin.IsUnlocked)
                {
                    binString += "1";
                }
                else
                {
                    binString += "0";
                }
            });

            CloudVariables.ImportantValues[3] = Convert.ToInt32(binString, 2);
            
            ServiceLocator.GetService<PlayGamesService>().SaveData();
            UIManager.Instance.UpdateUI();
        }
    }

    public void UpdateSkins()
    {
        var binString = Convert.ToString(CloudVariables.ImportantValues[3], 2);
        var array = binString.ToCharArray();
        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] == '1')
            {
                _skinsList?[i].UnlockSkin();
            }
        }
    }
}
