using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int Level;
    public int StopsCount;
    public float MaxSpeed;

    private void Awake()
    {
        Instance = this;
        Level = 0;
        StopsCount = 0;
        MaxSpeed = 6;
        MapGenerator.LevelUp += OnLevelUp;
        UpdateMaxSpeed();
    }

    private void OnLevelUp()
    {
        Level++;

        UpdateMaxSpeed();

        if (Level == 1)
        {
            StopsCount = 1;
        }

        if (Level % 5 == 0 && StopsCount < 5)
        {
            StopsCount++;
        }
    }

    private void UpdateMaxSpeed()
    {
        MaxSpeed = Mathf.Atan(Level / 12f + 0.63f) * 5.3f + 3 + GetPlayerSkillFactor();
    }

    private float GetPlayerSkillFactor()
    {
        return th(Level) * (GameData.LastLevel + GameData.BestScore) * Mathf.Exp(-Level * 0.05f) * 0.01f;
    }

    private float th(float x)
    {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
    }
}