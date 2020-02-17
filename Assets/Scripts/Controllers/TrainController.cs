﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class TrainController : MonoBehaviour
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
        
        protected SpriteRenderer _spriteRenderer;
        
        public Gradient gradient;

        public float gradientSmoothness;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(SetLastTrainPos());
            StartCoroutine(SetTrainColor());
        }
        
        private IEnumerator SetTrainColor()
        {
            while (true)
            {
                _spriteRenderer.color = gradient.Evaluate(Mathf.PingPong(Time.time/gradientSmoothness, 1));
                yield return null;
            }
        }

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
            transform.up = Vector3.Lerp(transform.up, vectorToTarget, 0.7f);
        }

        public virtual void MoveTrain(Vector2 vectorToTarget)
        {
            if (IsDead) return;
        
            transform.position = Vector2.MoveTowards(transform.position, NextTrain.LastTrainPos,
                15*Time.deltaTime);
        }
    
    
    }
}