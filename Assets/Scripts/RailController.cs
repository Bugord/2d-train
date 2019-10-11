﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.Scripts;
using UnityEngine;

[ExecuteInEditMode]
public class RailController : MonoBehaviour
{
    public RailDirection RailDirection;
    public List<Vector3> WayPoints;

    public int Row;
    public int InputId;
    public int OutputId;

    public SpriteRenderer _spriteRenderer;
    public SpriteMask _spriteMask;

    [SerializeField] private Sprite _fatSprite;
    [SerializeField] private Sprite _thinSprite;

    public RailController smallRail;

    public List<RailController> NextRails;
    public RailController NextActiveRail;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteMask = GetComponent<SpriteMask>();
    }

    private void Start()
    {
        NextRails = new List<RailController>();
    }

    public void SwitchRail()
    {
        if (NextRails.Count == 0) return;
        var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
        currentWayIndex++;
        if (currentWayIndex >= NextRails.Count)
            currentWayIndex = 0;
        NextActiveRail = NextRails[currentWayIndex];

        UpdateRailSprite();
        NextActiveRail.SwitchRail();
    }

    public void UpdateRailSprite()
    {
        if (NextActiveRail == null) return;

        var rails = MapGenerator._rowsList[NextActiveRail.Row].Rails;
        foreach (var rail in rails)
        {
            if (rail == NextActiveRail)
            {
                rail._spriteRenderer.sprite = rail._fatSprite;
                rail._spriteMask.enabled = true;
                rail.smallRail._spriteRenderer.sprite = rail.smallRail._fatSprite;
            }
            else
            {
                rail._spriteRenderer.sprite = rail._thinSprite;
                rail._spriteMask.enabled = false;
                rail.smallRail._spriteRenderer.sprite = rail.smallRail._thinSprite;
            }
        }
    }
}