﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Extentions;
using Assets.Scripts.Managers;
using Assets.Scripts.ObjectsPool;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class HeadTrainController : TrainController
    {
        public Vector3 TargetPoint;
        public int TargetPointIndex;
        public RailController TargetRail;

        private const float DistToChangeTarget = 0.1f;

        [SerializeField] private GameObject TrainPrefab;

        private List<Transform> TargetPointList;

        public bool Turning;

        [SerializeField] private GameObject _trailObject;

        private SpriteRenderer _spriteRenderer;

        [SerializeField] private GameObject _pointEffector;
    
        private void Start()
        {
            HeadTrain = this;
            Trains = new List<TrainController>();
            TargetPointList = TargetRail.WayPoints;
            TargetPoint = TargetPointList[0].localPosition;
            InputManager.Swipe += InputManagerOnSwipe;
            HeadTrain.Trains.Add(this);
            _adsService.TrainRevive += ReviveTrain;
            UIManager.Instance.GameRestart += ResetTrain;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SkinManager.Instance.UpdateSkin(_spriteRenderer);
            UpdateTrainPoints();
        }

#if UNITY_EDITOR
        private void Update()
        {
            var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;

            if (Input.GetKeyDown(KeyCode.Mouse0) || touch)
            {
                TargetRail.SwitchRail();
                _audioService.Play(AudioClipType.Swipe);
            }
        }
#endif

        private void FixedUpdate()
        {
            if (!UIManager.IsInGame) return;

            SetLastTrainPos();

            var vectorToTarget = VectorToTarget();

            SetRotation(vectorToTarget);
            MoveTrain(vectorToTarget);

            if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget)
            {
                ChangeTargetPoint();
            }
            UIManager.Instance._inGameUiController.Distance.text = GameData.Score.ToString();
        }

        private void InputManagerOnSwipe(SwipeDirection direction)
        {
#if !UNITY_EDITOR
        TargetRail.SwitchRail(direction);
        _audioService.Play(AudioClipType.Swipe);
#endif
        }


        private Vector2 VectorToTarget()
        {
            if (TargetRail == null)
            {
                return Vector2.zero;
            }
            return TargetPoint + TargetRail.transform.position - transform.position;
        }
    
        private void OnTriggerEnter2D(Collider2D col)
        {
            PoolObject poolObject = col.GetComponent<PoolObject>();
            if (col.tag == "Point")
            {
                if (IsBoosted)
                {
                    poolObject.ReturnToPool(0.5f);
                }
                else
                {
                    poolObject.ReturnToPool();
                }

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

                GameData.Coins++;
                UIManager.Instance._inGameUiController.Score.text = GameData.Coins.ToString();
                _achievementsService.UnlockAchievement(GPGSIds.achievement_first_coin);

                if (Trains != null)
                    _audioService.Play(AudioClipType.Coin, 0.5f + _levelService.GetSpeed()*0.05f);
            }
            else if (col.tag == "Boost")
            {
                poolObject.ReturnToPool();
                StartCoroutine(ActivateBoost());
                _achievementsService.UnlockAchievement(GPGSIds.achievement_speed_of_light);
            }
            else if (col.tag == "Stop" && !IsBoosted)
            {
                poolObject.ReturnToPool();
                if (Points > 2 * (Trains.Count - 1))
                {
                    Points -= Points - 2 * (Trains.Count - 1);
                }

                _levelService.ResetSpeed();

                var trainToRemove = Trains.Last();

                if (trainToRemove != this)
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
                _audioService.Play(AudioClipType.StopHit);
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

        private void ReviveTrain()
        {
            GameData.Revived = true;
            ResetTrain();
            UIManager.Instance.SetPause();
            UpdateTrainPoints();
        }

        private void ResetTrain()
        {
            _levelService.ResetSpeed();
            Points = 1;
            UpdateTrainPoints();
            GameObject.FindGameObjectsWithTag("Stop").ToList().ForEach(stop => stop.GetComponent<PoolObject>().ReturnToPool());
            UIManager.Instance.HideEndGameMenu();
        }

        private IEnumerator ActivateBoost()
        {
            _audioService.Play(AudioClipType.BoostStart);
            IsBoosted = true;
            _trailObject.SetActive(true);
            _pointEffector.SetActive(true);

            float t = 0;

            while (t <= 5)
            {
                t += UIManager.IsInGame ? Time.deltaTime : 0;
                yield return null;
            }

            _audioService.Play(AudioClipType.BoostEnd);
            IsBoosted = false;
            _trailObject.SetActive(false);
            _pointEffector.SetActive(false);
        }

        private void GenerateNewTrain()
        {
            var lastTrain = Trains.Last();
            var newTrainPos = lastTrain.LastTrainPos;
            var newTrain = Instantiate(TrainPrefab, newTrainPos, Quaternion.identity);
            var newTrainController = newTrain.GetComponent<TrainController>();
            newTrainController.NextTrain = lastTrain;
            HeadTrain.Trains.Add(newTrainController);
            newTrain.GetComponent<SpriteRenderer>().sprite = _spriteRenderer.sprite;
            _audioService.Play(AudioClipType.NewTrain);
        }

        private void ChangeTargetPoint(bool isLast = false)
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

            var railPoint = TargetPointList[isLast ? 0 : TargetPointIndex];

            TargetPoint = railPoint != null ? railPoint.localPosition : NextTrain.LastTrainPos;

            GameData.Score = TargetRail.Row;
        }

        public override void SetRotation(Vector2 vectorToTarget)
        {
            transform.up = vectorToTarget;
        }

        public override void MoveTrain(Vector2 vectorToTarget)
        {
            if (IsDead) return;

            var newSpeed = _levelService.GetSpeed();

            if (IsBoosted)
            {
                newSpeed = _levelService.BoostedSpeed;
            }

            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + vectorToTarget,
                newSpeed * Time.deltaTime);
        }
    }
}
