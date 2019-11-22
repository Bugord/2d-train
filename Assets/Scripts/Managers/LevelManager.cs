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
        Debug.LogError(GetPlayerSkillFactor());
    }

    private float GetPlayerSkillFactor()
    {
        return (GameData.LastLevel + GameData.BestScore*0.2f) * 0.5f * Mathf.Exp(-Level * 0.15f) * 0.1f;
    }
}