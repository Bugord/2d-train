using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;
    
    private SpriteRenderer _trainRenderer;

    public int RandomSkinCost;

    public void Awake()
    {
        Instance = this;
    }
    
    public void SetSkin(Sprite skin)
    {
        GameData.CurrentSkinId = skin.name;
        _trainRenderer.sprite = skin;
    }

    public void UpdateSkin(SpriteRenderer trainRenderer)
    {
        _trainRenderer = trainRenderer;
        _trainRenderer.sprite = LoadSkinFromResource(GameData.CurrentSkinId);
    }

    private Sprite LoadSkinFromResource(string skinId)
    {
        return Resources.Load<Sprite>($"Skins/{skinId}");
    }
}
