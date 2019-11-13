using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Extentions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainController : MonoBehaviour
{
    public float Speed;
    public float DefaultSpeed;
    public float MaxSpeed;
    public float SpeedStep;
    public float SpeedReduceStep;
    public float RotationSpeed;
    public Vector3 TargetPoint;
    public int TargetPointIndex;
    public RailController TargetRail;
    
    public bool Turning;

    public Sprite[] TrainPointSprites;

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
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsHeadTrain)
            return;
        Destroy(col.gameObject);

        if (col.tag == "Point")
        {

            GameData.InGameCoins++;
            Points++;
            if (Points > 2 * Trains.Count)
            {
                GenerateNewTrain();
                Points = 1;
            }

            for (var i = 0; i < Trains.Count; i++)
            {
                if (Points - 2 * i < 0)
                    Trains[i]._spriteRenderer.sprite = TrainPointSprites[0];
                else if (Points - 2 * i >= 2)
                    Trains[i]._spriteRenderer.sprite = TrainPointSprites[2];
                else
                    Trains[i]._spriteRenderer.sprite = TrainPointSprites[Points - 2 * i];
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
                Trains.ForEach(train => train.Speed -= SpeedReduceStep);
            }

            var trainToRemove = Trains.Last();
            Trains.Remove(trainToRemove);
            trainToRemove.Speed = Speed*0.3f;
            Destroy(trainToRemove.gameObject, 1.5f);
            if (Trains.Count == 0)
            {
                UIManager.Instance.ExitToMainMenu();
            }
        }
    }

    void GenerateNewTrain()
    {
        if (!IsHeadTrain)
            return;

        var lastTrain = Trains.Last();
        var newTrainPos = lastTrain.transform.position - Vector3.up*3;
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
        if(!UIManager.IsInGame)
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
        transform.rotation = Quaternion.Slerp(transform.rotation, q, RotationSpeed);
    }

    private void SetVelocity(Vector2 vectorToTarget)
    {
        if (Speed < MaxSpeed)
        {
            Speed += SpeedStep;
        }

        var newSpeed = Speed;

        if (!IsHeadTrain)
        {
            if (Vector3.Distance(transform.position, nextTrain.transform.position) > distanceBetweenTrains)
            {
                newSpeed = Speed * 1.3f;
            }
            if (Vector3.Distance(transform.position, nextTrain.transform.position) < distanceBetweenTrains)
            {
                newSpeed = Speed * 0.7f;
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