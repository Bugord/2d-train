using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.Scripts;
using UnityEngine;

[Serializable]
public struct WayStruct
{
    public Sprite Sprite;
    public Sprite Mask;
}

[ExecuteInEditMode]
public class RailController : MonoBehaviour
{
    public RailDirection RailDirection;
    public List<Vector3> WayPoints;
    public WayStruct CurrentWayStruct;

    public int Row;
    public int index;

    public SpriteRenderer _spriteRenderer;
    public SpriteMask _spriteMask;

    public List<RailController> NextRails;
    public RailController NextActiveRail;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteMask = GetComponent<SpriteMask>();
    }

    private void Start()
    {
        UpdateRailSprite();
        NextRails = new List<RailController>();
    }

    private void Update()
    {
        var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (Input.GetKeyDown(KeyCode.Mouse0) || touch)
        {
            var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
            currentWayIndex++;
            if (currentWayIndex >= 3)
                currentWayIndex = 0;
            RailDirection = (RailDirection)currentWayIndex;
            UpdateRailSprite();
        }
    }

    public void UpdateRailSprite()
    {
        _spriteRenderer.sprite = CurrentWayStruct.Sprite;
        _spriteMask.sprite = CurrentWayStruct.Mask;
    }
}