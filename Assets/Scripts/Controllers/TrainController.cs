using System;
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
    public Vector3 TargetPoint;
    public int TargetPointIndex;
    public RailController TargetRail;

    public bool Turning;

    public GameObject[] TrainPointSprites;

    public int Points = 1;
    public bool IsHeadTrain = false;
    public List<TrainController> Trains;

    public GameObject TrainPrefab;

    public List<Transform> TargetPointList;

    private Rigidbody2D _rigidbody2D;
    public SpriteRenderer _spriteRenderer;
    private const float DistToChangeTarget = 0.05f;

    public TrainController NextTrain;
    public static TrainController HeadTrain;
    public float distanceBetweenTrains;

    public RailController LastRail;

    public bool IsDead;
    public bool IsBoosted;
    public bool IsConnected;

    public static float Speed;

    [SerializeField] private GameObject _trailObject;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsHeadTrain)
            return;

        if (col.tag == "Point")
        {
            Destroy(col.gameObject);
            GameData.Coins++;
            UIManager.Instance._inGameUiController.Score.text = GameData.Coins.ToString();
            PlayGamesScript.UnlockAchievement(GPGSIds.achievement_first_coin);

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
        }else if(col.tag == "Boost")
        {
            PlayGamesScript.UnlockAchievement(GPGSIds.achievement_speed_of_light);
            Destroy(col.gameObject);
            StartCoroutine(ActivateBoost(GetComponent<CapsuleCollider2D>()));
        }else if (col.tag == "Stop" && !IsBoosted)
        {
            Destroy(col.gameObject);
            if (Points > 2 * (Trains.Count - 1))
            {
                Points -= Points - 2 * (Trains.Count - 1);
            }
            
            LevelManager.Instance.ResetSpeed();

            var trainToRemove = Trains.Last();

            if (!trainToRemove.IsHeadTrain)
            {
                Trains.Remove(trainToRemove);
                trainToRemove.IsDead = true;
                Destroy(trainToRemove.gameObject, 1.5f);
            }
            else
            {
                UIManager.IsInGame = false;
                UIManager.Instance.ShowEndGameMenu(true);
            }
        }
    }

    private void UpdateTrainPoints()
    {
        for (var i = 0; i < Trains.Count; i++)
        {
            if (Trains[i] == null) continue;
            
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
        newTrainController.NextTrain = lastTrain;
        newTrainController.IsBoosted = lastTrain.IsBoosted;
        Trains.First(controller => controller.IsHeadTrain).Trains.Add(newTrainController);
    }

    private void Awake()
    {
        Trains.Clear();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        InputManager.Swipe += InputManagerOnSwipe;
        if (!IsHeadTrain) return;
        if (HeadTrain == null)
        {
            HeadTrain = this;
        }
        Trains.Add(this);
        AdsManager.TrainRevive += ReviveTrain;
    }

    private void ReviveTrain()
    {
        LevelManager.Instance.ResetSpeed();
        Points = 1;
        UpdateTrainPoints();
        GameObject.FindGameObjectsWithTag("Stop").ToList().ForEach(Destroy);
        UIManager.Instance.HideEndGameMenu();
        UIManager.Instance.SetPause();
        UpdateTrainPoints();
    }

    private IEnumerator ActivateBoost(CapsuleCollider2D col)
    {
        IsBoosted = true;
        col.direction = CapsuleDirection2D.Horizontal;
        col.size = new Vector2(col.size.x*50, col.size.y);
        _trailObject.SetActive(true);

        float t = 0;

        while (t <= 5)
        {
            t += UIManager.IsInGame ? Time.deltaTime : 0;
            yield return null;
        }

        IsBoosted = false;
        col.direction = CapsuleDirection2D.Vertical;
        col.size = new Vector2(col.size.x / 50, col.size.y);
        _trailObject.SetActive(false);
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

        SetVelocity(vectorToTarget);
        SetRotation(vectorToTarget);

        if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget)
        {
            ChangeTargetPoint();
        }

        if (IsHeadTrain)
        {
            UIManager.Instance._inGameUiController.Distance.text = GameData.Score.ToString();
        }
    }

    private Vector2 VectorToTarget()
    {
        return TargetPoint + TargetRail.transform.position - transform.position;
    }

    private void SetRotation(Vector2 vectorToTarget)
    {
        transform.up = vectorToTarget;
    }

    private void SetVelocity(Vector2 vectorToTarget)
    {
        var newSpeed = !IsDead ? Speed : 0;
        if (!IsHeadTrain && !IsDead)
        {
            if (!IsConnected)
            {
                if (Vector3.Distance(transform.position, NextTrain.transform.position) > distanceBetweenTrains + 0.1f)
                {
                    newSpeed = Speed * 1.8f;
                }
                else
                {
                    IsConnected = true;
                    newSpeed = Speed;
                }
            }
            else
            {
                var distanceFactor = Vector3.Distance(transform.position, NextTrain.transform.position) - distanceBetweenTrains;
                if (distanceFactor <= 0.1f && distanceFactor >= -0.1f)
                {
                    newSpeed = Speed;
                }

                if (distanceFactor < -distanceBetweenTrains*0.1f)
                {
                    newSpeed = Speed * 0.9f;
                }
                if (distanceFactor > distanceBetweenTrains * 0.1f && _rigidbody2D.velocity.magnitude < NextTrain._rigidbody2D.velocity.magnitude)
                {
                    newSpeed = distanceFactor > distanceBetweenTrains ? Speed * 1.1f : Speed * 1.8f;
                }
            }
        }

        if (HeadTrain.IsBoosted)
        {
            newSpeed = LevelManager.Instance.BoostedSpeed;
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

        if (!IsHeadTrain) return;
        
        GameData.Score = TargetRail.Row;
    }
}