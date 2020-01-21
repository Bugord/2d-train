using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts;
using Assets.Scripts.Extentions;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

public class TrainController : MonoBehaviour
{
    public GameObject[] TrainPointSprites;

    public int Points = 1;
    
    public List<TrainController> Trains;
    public Transform LookAtTarget;
    
    public TrainController NextTrain;
    public static TrainController HeadTrain;
    public float distanceBetweenTrains;
    
    public bool IsDead;
    public bool IsBoosted;
    
    public Vector3 LastTrainPos;
    private List<Vector3> _points;

    public static float Speed;

    public float sectorAngle = 90;
    public float step = 1;
    
    private int _railLayer;
    
    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.layer == _railLayer)
        {
            if (coll.gameObject.GetComponent<RailController>().IsActive)
            {
                SetLastTrainPos(coll);
            }
        }
    }

    private void Awake()
    {
        _railLayer = LayerMask.NameToLayer("Rail");
        LastTrainPos = transform.position;
        _points = new List<Vector3>();
    }
    
    private void FixedUpdate()
    {
        if (!UIManager.IsInGame) return;

        var vectorToTarget = NextTrain.LastTrainPos;

        SetRotation(NextTrain.LookAtTarget.position - transform.position);
        MoveTrain(vectorToTarget);
    }

    public void SetLastTrainPos(Collider2D coll)
    {
        _points.Clear();
        var backDirectionAngle = transform.eulerAngles.z + 270;

        for (var angle = backDirectionAngle - sectorAngle * 0.5f; angle < backDirectionAngle + sectorAngle * 0.5f; angle += step)
        {
            var newPos = transform.position + new Vector3(distanceBetweenTrains * Mathf.Cos(angle * Mathf.Deg2Rad), distanceBetweenTrains * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            
            if (coll.OverlapPoint(newPos))
            {
                _points.Add(newPos);
            }
        }

        if (_points.Count > 1)
        {
            LastTrainPos = _points[(int)(_points.Count * 0.5f)];
        }

        if (_points.Count == 1)
        {
            LastTrainPos = _points.First();
        }
    }
    
    public virtual void SetRotation(Vector2 vectorToTarget)
    {
        transform.up = vectorToTarget;
    }

    public virtual void MoveTrain(Vector2 vectorToTarget)
    {
        if (IsDead) return;

        var newSpeed = Speed;
        
        if (HeadTrain.IsBoosted)
        {
            newSpeed = LevelManager.Instance.BoostedSpeed;
        }
        
        newSpeed *= 1.5f;
        transform.position = Vector2.MoveTowards(transform.position, NextTrain.LastTrainPos,
            newSpeed * Time.deltaTime);
    }
    
    
}