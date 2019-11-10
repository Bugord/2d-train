﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : PanelBase
{
    public Button StartButton;
    public Button SettingButton;
    public Button ShopButton;

    public Text BestScore;
    public Text Coins;
    
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        Debug.LogError("Start");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
