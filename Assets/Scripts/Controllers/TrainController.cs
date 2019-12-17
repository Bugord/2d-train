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
    
    public Vector3 LastTrainPos;

    public static float Speed;

    [SerializeField] private GameObject _trailObject;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsHeadTrain)
            return;

        if (col.tag == "Point")
        {
            if (IsBoosted)
            {
               Destroy(col.gameObject, 0.75f); 
            }
            else
            {
                Destroy(col.gameObject);
            }
            
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
        var newTrainPos = lastTrain.transform.position + Vector3.down * 3;
        var newTrain = Instantiate(TrainPrefab, newTrainPos, Quaternion.identity);
        Destroy(newTrain.GetComponent<CapsuleCollider2D>());
        var newTrainController = newTrain.GetComponent<TrainController>();
        newTrainController.TargetRail = lastTrain.LastRail;
        newTrainController.IsHeadTrain = false;
        newTrainController.NextTrain = lastTrain;
        newTrainController.IsBoosted = lastTrain.IsBoosted;
        Trains.First(controller => controller.IsHeadTrain).Trains.Add(newTrainController);
        Destroy(newTrainController._trailObject);
    }

    private void Awake()
    {
        LastTrainPos = transform.position;
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
        _trailObject.SetActive(true);

        float t = 0;

        while (t <= 5)
        {
            t += UIManager.IsInGame ? Time.deltaTime : 0;
            yield return null;
        }

        IsBoosted = false;
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
        if (!IsHeadTrain)
        {
            return;
        }
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

    public float TimeToNextTrain;
    private void FixedUpdate()
    {
        if (!UIManager.IsInGame) return;

        var vectorToTarget = VectorToTarget();

        SetRotation(vectorToTarget);
        MoveTrain(vectorToTarget);
        StartCoroutine(SetLastTrainPos( 37 / (IsBoosted ? LevelManager.Instance.BoostedSpeed : Speed)* Time.fixedDeltaTime, transform.position));

        if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget && IsHeadTrain)
        {
            ChangeTargetPoint();
        }

        if (IsHeadTrain)
        {
            UIManager.Instance._inGameUiController.Distance.text = GameData.Score.ToString();
        }
    }

    private IEnumerator SetLastTrainPos(float time, Vector3 pos)
    {
        yield return new WaitForSeconds(time);

        LastTrainPos = pos;
    }

    private Vector2 VectorToTarget()
    {
        return (IsHeadTrain ? TargetPoint + TargetRail.transform.position : NextTrain.LastTrainPos)- transform.position;
    }

    private void SetRotation(Vector2 vectorToTarget)
    {
        transform.up = vectorToTarget;
    }

    private void MoveTrain(Vector2 vectorToTarget)
    {
        if (IsDead) return;

        var newSpeed = Speed;
        
        if (HeadTrain.IsBoosted)
        {
            newSpeed = LevelManager.Instance.BoostedSpeed;
        }

        if (IsHeadTrain)
        {
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + vectorToTarget,
                newSpeed * Time.deltaTime);
            return;
        }
        var distance = Vector3.Distance(transform.position, NextTrain.transform.position);
        if (!IsConnected)
        {
            if (distance > distanceBetweenTrains + 0.1f)
            {
                newSpeed = Speed * 1.5f;
            }
            else
            {
                IsConnected = true;
                newSpeed = Speed;
            }

            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + vectorToTarget,
                newSpeed * Time.deltaTime);
        }
        else
        {
            if (distance < distanceBetweenTrains)
            {
                newSpeed = 0;
            }
            transform.position = Vector2.MoveTowards(transform.position, NextTrain.LastTrainPos,
                newSpeed * Time.deltaTime); ;
        }

    }

    private void ChangeTargetPoint()
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
        TargetPoint = TargetPointList[TargetPointIndex].localPosition;

        if (!IsHeadTrain) return;
        
        GameData.Score = TargetRail.Row;
    }
}