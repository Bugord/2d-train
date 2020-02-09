﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ScrollRectSnappingController : MonoBehaviour
    {
        float[] points;
     [Tooltip("how many screens or pages are there within the content (steps)")]
     public int screens = 1;

     public List<RectTransform> _pages;
     public List<Image> _indicators;
     [SerializeField] private Sprite _activePoint;
     [SerializeField] private Sprite _inactivePoint;
     float stepSize;
 
     ScrollRect scroll;
     bool LerpH;
     float targetH;
     [Tooltip("Snap horizontally")]
     public bool snapInH = true;
 
     bool LerpV;
     float targetV;
     [Tooltip("Snap vertically")]
     public bool snapInV = true;
 
     // Use this for initialization
     void Start ()
     {
         scroll = gameObject.GetComponent<ScrollRect>();
         
         screens = _pages.Count;
         
         if(screens > 0)
         {
             points = new float[screens];
             stepSize = 1/(float)(screens-1);
         
             for(int i = 0; i < screens; i++)
             {
                 points[i] = i * stepSize;
             }
         }
         else
         {
             points[0] = 0;
         }
     }
     
     void Update()
     {
         if(LerpH)
         {
             scroll.horizontalNormalizedPosition = Mathf.Lerp( scroll.horizontalNormalizedPosition, targetH, 10*scroll.elasticity*Time.deltaTime);
             if(Mathf.Approximately(scroll.horizontalNormalizedPosition, targetH)) LerpH = false;            
         }
         
         var index = screens * Math.Abs(scroll.horizontalScrollbar.value);
         index -= index % 1;
         if (index > screens-1)
         {
             index = screens - 1;
         }
         for (var i = 0; i < _indicators.Count; i++)
         {
             _indicators[i].sprite = i == (int) index ? _activePoint : _inactivePoint;
         }
         
         if(LerpV)
         {
             scroll.verticalNormalizedPosition = Mathf.Lerp( scroll.verticalNormalizedPosition, targetV, 10*scroll.elasticity*Time.deltaTime);
             if(Mathf.Approximately(scroll.verticalNormalizedPosition, targetV)) LerpV = false;            
         }
     }
 
     public void DragEnd()
     {
         if(scroll.horizontal && snapInH)
         {
             targetH = points[FindNearest(scroll.horizontalNormalizedPosition, points)];
             LerpH = true;
         }
         if(scroll.vertical && snapInV)
         {
             targetH = points[FindNearest(scroll.verticalNormalizedPosition, points)];
             LerpH = true;
         }
     }
 
     public void OnDrag()
     {
         LerpH = false;
         LerpV = false;
     }
 
     int FindNearest(float f, float[] array)
     {
         float distance = Mathf.Infinity;
         int output = 0;
         for(int index = 0; index < array.Length; index++)
         {
             if(Mathf.Abs(array[index]-f) < distance)
             {
                 distance = Mathf.Abs(array[index]-f);
                 output = index;
             }
         }
         return output;    
     }
    }
}
