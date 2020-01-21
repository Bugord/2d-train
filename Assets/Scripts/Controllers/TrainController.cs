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
    private List<Vector3> _lastPoints;

    public static float Speed;

    private void Awake()
    {
        LastTrainPos = transform.position;
        _lastPoints = new List<Vector3>();
    }
    
    private void FixedUpdate()
    {
        if (!UIManager.IsInGame) return;
        
        SetLastTrainPos();

        SetRotation(NextTrain.LookAtTarget.position - transform.position);
        MoveTrain(NextTrain.LastTrainPos);
    }

    public void SetLastTrainPos()
    {
        _lastPoints.Add(transform.position);
        _lastPoints.RemoveAll(p => Vector3.Distance(transform.position, p) > distanceBetweenTrains);
        LastTrainPos = _lastPoints.First();
    }
    
    public virtual void SetRotation(Vector2 vectorToTarget)
    {
        transform.up = vectorToTarget;
    }

    public virtual void MoveTrain(Vector2 vectorToTarget)
    {
        if (IsDead) return;
        
        transform.position = NextTrain.LastTrainPos;
    }
    
    
}