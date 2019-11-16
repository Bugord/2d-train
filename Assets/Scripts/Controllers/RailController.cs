using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Assets.Scripts;
using UnityEngine;

[ExecuteInEditMode]
public class RailController : MonoBehaviour
{
    public RailDirection RailDirection;
    public List<Transform> WayPoints;

    public int Row;
    public int InputId;
    public int OutputId;

    public SpriteRenderer _spriteRenderer;
    public SpriteMask _spriteMask;

    [SerializeField] private Sprite _fatSprite;
    [SerializeField] private Sprite _thinSprite;
    public Sprite _splitMask;
    
    public List<RailController> NextRails;
    public RailController NextActiveRail;

    public Transform EndPoint;

    public Transform PointPosition;

    public bool IsActive;
    public bool HasStop;
    
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
        NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();
        
        var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
        currentWayIndex++;
        if (currentWayIndex >= NextRails.Count)
            currentWayIndex = 0;
        NextActiveRail = NextRails[currentWayIndex];

        UpdateRailSprite();
        NextActiveRail.SwitchRail();
    }

    public void SwitchRail(SwipeDirection direction)
    {
        if (NextRails.Count == 0) return;
        NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();
        
        var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
        if (direction == SwipeDirection.Left)
        {
            currentWayIndex--;
        }

        if (direction == SwipeDirection.Right)
        {
            currentWayIndex++;
        }

        if (currentWayIndex >= NextRails.Count)
            currentWayIndex = NextRails.Count - 1;

        if (currentWayIndex < 0)
            currentWayIndex = 0;

        NextActiveRail = NextRails[currentWayIndex];
    
        UpdateRailSprite();
        NextActiveRail.SwitchRail(direction);
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
                rail.IsActive = true;
            }
            else
            {
                rail._spriteRenderer.sprite = rail._thinSprite;
                rail._spriteMask.enabled = false;
                rail.IsActive = false;
            }
        }
    }
}