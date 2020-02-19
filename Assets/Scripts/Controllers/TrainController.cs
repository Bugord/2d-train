﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class TrainController : PoolObject
    {
        public GameObject[] TrainPointSprites;

        public int Points = 1;
        
        public Transform LookAtTarget;
    
        public TrainController NextTrain;
        public float distanceBetweenTrains;
    
        public bool IsDead;
        public bool IsBoosted;
    
        public Vector3 LastTrainPos;
        private List<Vector3> _lastPoints = new List<Vector3>();
        
        public SpriteRenderer spriteRenderer;
        
        public Gradient gradient;

        public float gradientSmoothness;

        public float speed;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(SetLastTrainPos());
            //StartCoroutine(SetTrainColor());
        }
        
        // private IEnumerator SetTrainColor()
        // {
        //     while (true)
        //     {
        //         _spriteRenderer.color = gradient.Evaluate(Mathf.PingPong(Time.time/gradientSmoothness, 1));
        //         yield return null;
        //     }
        // }

        private void FixedUpdate()
        {
            SetRotation(NextTrain.LookAtTarget.position - transform.position);
            MoveTrain(NextTrain.LastTrainPos);
        }
       
        public IEnumerator SetLastTrainPos()
        {
            while (true)
            {
                _lastPoints.Add(transform.position);
                _lastPoints.RemoveAll(p => Vector3.Distance(transform.position, p) >= distanceBetweenTrains);
                LastTrainPos = _lastPoints.First();
             
                yield return null;
            }
        }
    
        public void SetRotation(Vector2 vectorToTarget)
        {
            transform.up = Vector3.Lerp(transform.up, vectorToTarget, 0.85f);
        }

        public virtual void MoveTrain(Vector2 vectorToTarget)
        {
            if (IsDead) return;

            speed = NextTrain.speed;
            
            transform.position = Vector2.MoveTowards(transform.position, NextTrain.LastTrainPos,
                speed*1.225f*Time.fixedDeltaTime);
        }
    }
}