using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[ExecuteInEditMode]
public class RailController : MonoBehaviour
{
    public Sprite[] Sprites;
    public Sprite[] Masks;
    [Range(0, 2)] public int TypeRail;

    public Vector3[] CommonPoints;

    [Serializable]
    public class PointList
    {
        public Vector3[] Points;
    }

    public PointList[] PointLists;

    public List<RailController> NextRailControllers;
    public RailController LeftRailControllers;
    public RailController MiddleRailControllers;
    public RailController RightRailControllers;
    public bool TurnsLeft;
    public bool TurnsMiddle;
    public bool TurnsRight;
    public int Row;
    public int index;

    public SpriteMask SpriteMask;

    public bool IsDragged;

    public SpriteRenderer _spriteRenderer;

    private void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SpriteMask = transform.GetChild(0).GetComponent<SpriteMask>();
    }

    public void Enable()
    {
        NextRailControllers = new List<RailController>();
        if (LeftRailControllers != null && TurnsLeft)
            NextRailControllers.Add(LeftRailControllers);
        if (MiddleRailControllers != null && TurnsMiddle)
            NextRailControllers.Add(MiddleRailControllers);
        if (RightRailControllers != null && TurnsRight)
            NextRailControllers.Add(RightRailControllers);
    }

    void Update()
    {
        var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (!Input.GetKeyDown(KeyCode.Mouse0) && !touch) return;
        TypeRail++;
        if (TypeRail >= Sprites.Length)
            TypeRail = 0;
        UpdateRailSprite();
    }

    public void UpdateRailSprite()
    {
        if (TypeRail >= Sprites.Length)
            TypeRail = Sprites.Length - 1;
        if (TypeRail < 0)
            TypeRail = 0;

        _spriteRenderer.sprite = Sprites[TypeRail];
        SpriteMask.sprite = Masks[TypeRail];
    }

    private void OnValidate()
    {
        UpdateRailSprite();
    }
}