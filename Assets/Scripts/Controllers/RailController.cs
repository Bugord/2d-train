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

    public void SwitchRail(bool isFirst = true)
    {
        if (NextRails.Count == 0) return;
        NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();

        bool normalVariant = true;

        if (isFirst)
        {
            var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
            currentWayIndex++;
            if (currentWayIndex >= NextRails.Count)
                currentWayIndex = 0;
            NextActiveRail = NextRails[currentWayIndex];
        }
        else
        {
            if (NextRails.TrueForAll(rail => !rail.transform.Find("Stop_Line(Clone)")))
            {
                var currentWayIndex = NextRails.FindIndex(way => way == NextActiveRail);
                currentWayIndex++;
                if (currentWayIndex >= NextRails.Count)
                    currentWayIndex = 0;
                NextActiveRail = NextRails[currentWayIndex];
            }
            else
            {
                NextActiveRail = NextRails.First(rail => rail.transform.Find("Stop_Line(Clone)"));
                normalVariant = false;
            }
        }

        UpdateRailSprite();
        NextActiveRail.SwitchRail(normalVariant);
    }

    public void SwitchRail(SwipeDirection direction, bool isFirst = true)
    {
        if (NextRails.Count == 0) return;
        NextRails = NextRails.OrderBy(rail => rail.OutputId).ToList();

        bool normalVariant = true;

        if (isFirst)
        {
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
        }
        else
        {
            if (NextRails.TrueForAll(rail => !rail.transform.Find("Stop_Line(Clone)")))
            {
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
            }
            else
            {
                NextActiveRail = NextRails.First(rail => rail.transform.Find("Stop_Line(Clone)"));
                normalVariant = false;
            }
        }

        UpdateRailSprite();
        NextActiveRail.SwitchRail(direction, normalVariant);
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
                rail._spriteRenderer.color = Color.white;
                rail._spriteRenderer.sortingOrder = 2;
                rail._spriteMask.enabled = true;
                rail.IsActive = true;
            }
            else
            {
                rail._spriteRenderer.sprite = rail._thinSprite;
                float h, s, v;
                Color.RGBToHSV(Camera.main.backgroundColor, out h, out s, out v);
                rail._spriteRenderer.color = Color.HSVToRGB(h, s +0.1f, v - 0.45f);
                rail._spriteRenderer.sortingOrder = 0;
                rail._spriteMask.enabled = false;
                rail.IsActive = false;
            }
        }
    }
}