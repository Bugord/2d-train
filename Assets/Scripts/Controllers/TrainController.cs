﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Extentions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

public class TrainController : MonoBehaviour
{
    public float Speed;
    public float DefaultSpeed;
    public float SpeedStep;

    public float RotationSpeed;
    public Vector3 TargetPoint;
    public int TargetPointIndex;
    public RailController TargetRail;

    public bool Turning;

    public GameObject[] TrainPointSprites;

    public int Points = 1;
    public bool IsHeadTrain;
    public List<TrainController> Trains;

    public GameObject TrainPrefab;

    public List<Transform> TargetPointList;

    private Rigidbody2D _rigidbody2D;
    public SpriteRenderer _spriteRenderer;
    private const float DistToChangeTarget = 0.05f;

    public TrainController nextTrain;
    public float distanceBetweenTrains;

    public RailController LastRail;

    public float step = 0;
    public bool IsDead;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsHeadTrain)
            return;
        Destroy(col.gameObject);

        if (col.tag == "Point")
        {
            GameData.SetInGameCoins();

            if (Trains.Count <= 5)
            {
                Points++;
                if (Points > 2 * Trains.Count && Trains.Count < 5)
                {
                    GenerateNewTrain();
                    Points = 1;
                }

                UpdateTrainPoints();
            }
        }
        else if (col.tag == "Stop")
        {
            if (Points > 2 * (Trains.Count - 1))
            {
                Points -= Points - 2 * (Trains.Count - 1);
            }

            if (Speed > DefaultSpeed)
            {
                Trains.ForEach(train =>
                {
                    train.Speed = DefaultSpeed;
                    train.step = 0;
                });
            }

            var trainToRemove = Trains.Last();

            if (!trainToRemove.IsHeadTrain)
            {
                Trains.Remove(trainToRemove);
                trainToRemove.Speed = Speed * 0.1f;
                trainToRemove.IsDead = true;
                Destroy(trainToRemove.gameObject, 1.5f);
            }
            else
            {
                UIManager.IsInGame = false;
                UIManager.Instance.ShowEndGameMenu(true);
            }

            if (Trains.Count == 0)
            {
                UIManager.Instance.ShowEndGameMenu(true);
            }
        }
    }

    private void UpdateTrainPoints()
    {
        for (var i = 0; i < Trains.Count; i++)
        {
            if (Points - 2 * i < 0)
            {
                Trains[i].TrainPointSprites[0].SetActive(false);
                Trains[i].TrainPointSprites[1].SetActive(false);
            }
            else if (Points - 2 * i >= 2)
            {
                Trains[i].TrainPointSprites[0].SetActive(true);
                Trains[i].TrainPointSprites[1].SetActive(true);
            }
            else
            {
                switch (Points - 2 * i)
                {
                    case 0:
                        Trains[i].TrainPointSprites[0].SetActive(false);
                        Trains[i].TrainPointSprites[1].SetActive(false);
                        break;

                    case 1:
                        Trains[i].TrainPointSprites[0].SetActive(true);
                        Trains[i].TrainPointSprites[1].SetActive(false);
                        break;
                }
            }
        }
    }

    private void GenerateNewTrain()
    {
        if (!IsHeadTrain)
            return;

        var lastTrain = Trains.Last();
        var newTrainPos = lastTrain.LastRail.transform.position;
        var newTrain = Instantiate(TrainPrefab, newTrainPos, Quaternion.identity);
        Destroy(newTrain.GetComponent<CapsuleCollider2D>());
        var newTrainController = newTrain.GetComponent<TrainController>();
        newTrainController.TargetRail = lastTrain.LastRail;
        newTrainController.ChangeTargetPoint(true);
        newTrainController.IsHeadTrain = false;
        newTrainController.nextTrain = lastTrain;
        Trains.First(controller => controller.IsHeadTrain).Trains.Add(newTrainController);
    }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        InputManager.Swipe += InputManagerOnSwipe;
        if (!IsHeadTrain) return;
        Trains.Add(this);
        AdsManager.TrainRevive += ReviveTrain;
    }

    private void ReviveTrain()
    {
        Speed = DefaultSpeed;
        Points = 0;
        UpdateTrainPoints();
        UIManager.Instance.HideEndGameMenu();
        UIManager.Instance.SetPause();
        GameObject.FindGameObjectsWithTag("Stop").ToList().ForEach(Destroy);
    }

    private void InputManagerOnSwipe(SwipeDirection direction)
    {
#if !UNITY_EDITOR
        if (IsHeadTrain)
        {
            TargetRail.SwitchRail(direction);
        }
#endif
    }

    private void Start()
    {
        TargetPointList = TargetRail.WayPoints;
        TargetPoint = TargetPointList[0].localPosition;
    }

    private void Update()
    {
        if (!IsHeadTrain) return;

        GameData.SetLastScore(TargetRail.Row);
#if UNITY_EDITOR
        var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        if ((Input.GetKeyDown(KeyCode.Mouse0) || touch) && IsHeadTrain)
        {
            TargetRail.SwitchRail();
        }
#endif
    }

    private void FixedUpdate()
    {
        if (!UIManager.IsInGame)
        {
            StopTheTrain();
            return;
        }

        var vectorToTarget = VectorToTarget();

        SetRotation(vectorToTarget);
        SetVelocity(vectorToTarget);

        if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget)
            ChangeTargetPoint();
    }

    private Vector2 VectorToTarget()
    {
        return TargetPoint + TargetRail.transform.position - transform.position;
    }

    private void SetRotation(Vector2 vectorToTarget)
    {
        var angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        var q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, q, RotationSpeed);
    }

    private void SetVelocity(Vector2 vectorToTarget)
    {
        if (Speed < LevelManager.Instance.MaxSpeed && !IsDead)
        {
            Speed = Speed + (LevelManager.Instance.MaxSpeed - DefaultSpeed) * Mathf.Atan(Mathf.Lerp(0, Mathf.PI * 0.5f, step));
            step += SpeedStep;
        }

        var newSpeed = Speed;

        if (!IsHeadTrain)
        {
            if (Vector3.Distance(transform.position, nextTrain.transform.position) > distanceBetweenTrains)
            {
                newSpeed = Speed * 1.2f;
            }
            if (Vector3.Distance(transform.position, nextTrain.transform.position) > distanceBetweenTrains + 1)
            {
                newSpeed = Speed * 1.7f;
            }
            if (Vector3.Distance(transform.position, nextTrain.transform.position) < distanceBetweenTrains)
            {
                newSpeed = Speed * 0.8f;
            }
            if (Math.Abs(Vector3.Distance(transform.position, nextTrain.transform.position) - distanceBetweenTrains) <= 0)
            {
                newSpeed = Speed;
            }
        }

        _rigidbody2D.velocity = newSpeed * Vector3.Normalize(vectorToTarget);
    }

    private void StopTheTrain()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }

    private void ChangeTargetPoint(bool lastPoint = false)
    {
        TargetPointIndex++;
        if (TargetPointIndex >= TargetPointList.Count)
        {
            TargetPointIndex = 0;
            if (TargetRail.NextActiveRail == null)
            {
                if (!Turning && !TargetRail.WayPoints.IsNullOrEmpty())
                {
                    Turning = true;
                    TargetPointList = TargetRail.WayPoints;
                }
                else
                {
                    Turning = false;
                    TargetPointList = TargetRail.WayPoints;
                }
            }
            else
            {
                LastRail = TargetRail;
                TargetRail = TargetRail.NextActiveRail;
                TargetPointList = TargetRail.WayPoints;
                var last = TargetRail;
                while (last != null)
                {
                    last.UpdateRailSprite();
                    last = last.NextActiveRail;
                }
            }
        }
        TargetPoint = TargetPointList[lastPoint ? TargetPointList.Count - 1 : TargetPointIndex].localPosition;
    }
}