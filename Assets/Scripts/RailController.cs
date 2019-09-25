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
    public Way Way;
    public List<Vector3> WayPoints;
    public RailController WayRailController;
    public Sprite Sprite;
    public Sprite Mask;
}

[ExecuteInEditMode]
public class RailController : MonoBehaviour
{
    public Way Way;
    public List<Vector3> CommonPoints;
    public WayStruct CurrentWayStruct;

    public int Row;
    public int index;

    public SpriteRenderer _spriteRenderer;
    public SpriteMask _spriteMask;

    public List<WayStruct> WayStructs;

    public RailController NextRail;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteMask = GetComponent<SpriteMask>();
        CurrentWayStruct = WayStructs[0];
    }

    private void Start()
    {
        UpdateRailSprite();
    }

    private void Update()
    {
        var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (Input.GetKeyDown(KeyCode.Mouse0) || touch)
        {
            var currentWayIndex = (int)Way;
            currentWayIndex++;
            if (currentWayIndex >= 3)
                currentWayIndex = 0;
            Way = (Way)currentWayIndex;
            UpdateRailSprite();
        }
    }

    public void UpdateRailSprite()
    {
        _spriteRenderer.sprite = CurrentWayStruct.Sprite;
        _spriteMask.sprite = CurrentWayStruct.Mask;
    }
}