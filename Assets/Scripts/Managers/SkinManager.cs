using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public Sprite CurrentSkin;

    private SpriteRenderer _trainRenderer;

    public int RandomSkinCost;

    public void Awake()
    {
        Instance = this;
    }

    public void SetSkin(Sprite skin)
    {
        CurrentSkin = skin;
        _trainRenderer.sprite = skin;
    }

    public void UpdateSkin(SpriteRenderer renderer)
    {
        _trainRenderer = renderer;
        _trainRenderer.sprite = CurrentSkin;
    }
}
