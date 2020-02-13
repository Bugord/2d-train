using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Extentions;
using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
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

        public List<TrainController> Trains;

        private List<Transform> TargetPointList;

        public bool Turning;

        [SerializeField] private GameObject _trailObject;
        
        [SerializeField] private GameObject _pointEffector;
        [SerializeField] private GameObject _particleSystem;
        
        private AchievementsService _achievementsService;
        private AudioService _audioService;
        private LevelService _levelService;
        private AdsService _adsService;
        private SkinService _skinService;
        private GameDataService _gameDataService;
        private UIService _uiService;
        private bool IsFirstTime;

        private void Awake()
        {
            LastTrainPos = transform.position;
            _achievementsService = ServiceLocator.GetService<AchievementsService>();
            _audioService = ServiceLocator.GetService<AudioService>();
            _levelService = ServiceLocator.GetService<LevelService>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _skinService = ServiceLocator.GetService<SkinService>();
            _gameDataService = ServiceLocator.GetService<GameDataService>();
            _uiService = ServiceLocator.GetService<UIService>();

            Trains = new List<TrainController>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            TargetPointList = TargetRail.WayPoints;
            TargetPoint = TargetPointList[0].localPosition;
            Trains.Add(this);

            InputManager.Swipe += InputManagerOnSwipe;
            _adsService.TrainRevive += ReviveTrain;
            _uiService.GameRestart += ResetTrain;

            _skinService.UpdateSkin(_spriteRenderer);
            UpdateTrainPoints();
            IsFirstTime = !PlayerPrefs.HasKey("FirstTime");
        }

        private void OnDestroy()
        {
            InputManager.Swipe -= InputManagerOnSwipe;
            _adsService.TrainRevive -= ReviveTrain;
            _uiService.GameRestart -= ResetTrain;
        }

#if UNITY_EDITOR
        private void Update()
        {
            _spriteRenderer.color = gradient.Evaluate(Mathf.PingPong(Time.time/gradientSmoothness, 1));
            if (!_uiService.IsInGame) return;
            
            var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;

            if (Input.GetKeyDown(KeyCode.Mouse0) || touch)
            {
                TargetRail.SwitchRail();
                _audioService.Play(AudioClipType.Swipe);
                _uiService.ShowTutorial(false);
            }
        }
#endif

        private void FixedUpdate()
        {
            if (!_uiService.IsInGame || _uiService.IsInTutorial) return;
            if (IsFirstTime)
            {
                if (TargetRail.NextActiveRail == MapGenerator.Instance.tutorialRail &&
                    Vector2.Distance(MapGenerator.Instance.tutorialRail.transform.position, transform.position) < 2)
                {
                    _uiService.ShowTutorial(true);
                    IsFirstTime = false;
                    PlayerPrefs.SetString("FirstTime", "");
                }
            }

            SetLastTrainPos();

            var vectorToTarget = VectorToTarget();

            SetRotation(vectorToTarget);
            MoveTrain(vectorToTarget);

            if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget)
            {
                ChangeTargetPoint();
            }
            _uiService.UpdateInGameDistance(_gameDataService.Score);
        }

        private void InputManagerOnSwipe(SwipeDirection direction)
        {
#if !UNITY_EDITOR
        if (!_uiService.IsInGame) return;
        TargetRail.SwitchRail(direction);
        _audioService.Play(AudioClipType.Swipe);
        _uiService.ShowTutorial(false);
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
            if (col.CompareTag("Point"))
            {
                if (IsBoosted)
                {
                    poolObject.ReturnToPool(0.5f);
                }
                else
                {
                    poolObject.ReturnToPool();
                }
                
                Points++;
                if (Points > 2 * Trains.Count)
                {
                    GenerateNewTrain();
                    Points = 1;
                }

                UpdateTrainPoints();

                _gameDataService.Coins++;
                _uiService.UpdateInGameCoins(_gameDataService.Coins);
                _achievementsService.UnlockAchievement(GPGSIds.achievement_first_coin);

                if (Trains != null)
                    _audioService.Play(AudioClipType.Coin, 0.5f + _levelService.GetSpeed()*0.05f);
            }
            else if (col.CompareTag("Boost"))
            {
                poolObject.ReturnToPool();
                StartCoroutine(ActivateBoost());
                _achievementsService.UnlockAchievement(GPGSIds.achievement_speed_of_light);
            }
            else if (col.CompareTag("Stop") && !IsBoosted)
            {
                poolObject.ReturnToPool();
                
                Points = 1;
                UpdateTrainPoints();

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
                    _uiService.IsInGame = false;
                    _uiService.ShowReviveMenu(!_gameDataService.Revived);
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
            _gameDataService.Revived = true;
            ResetTrain();
            _uiService.SetPause();
            UpdateTrainPoints();
        }

        private void ResetTrain()
        {
            _levelService.ResetSpeed();
            Points = 1;
            UpdateTrainPoints();
            Trains.ForEach(train =>
            {
                if (train == this) return;
                Destroy(train.gameObject);
            });

            Trains.Clear();
            Trains.Add(this);

            IsBoosted = false;
            _trailObject.SetActive(false);

            GameObject.FindGameObjectsWithTag("Stop").ToList().ForEach(stop => stop.GetComponent<PoolObject>().ReturnToPool());
            GameObject.FindGameObjectsWithTag("Boost").ToList().ForEach(boost => boost.GetComponent<PoolObject>().ReturnToPool());
        }

        private IEnumerator ActivateBoost()
        {
            _audioService.Play(AudioClipType.BoostStart);
            IsBoosted = true;
            _trailObject.SetActive(true);
            _pointEffector.SetActive(true);
            _particleSystem.SetActive(true);

            float t = 0;

            while (t <= 5)
            {
                t += _uiService.IsInGame ? Time.deltaTime : 0;
                yield return null;
            }

            _audioService.Play(AudioClipType.BoostEnd);
            IsBoosted = false;
            _trailObject.SetActive(false);
            _pointEffector.SetActive(false);
            _particleSystem.SetActive(false);
        }

        private void GenerateNewTrain()
        {
            var lastTrain = Trains.Last();
            var newTrainPos = lastTrain.LastTrainPos;
            var newTrain = Instantiate(TrainPrefab, newTrainPos, Quaternion.identity);
            var newTrainController = newTrain.GetComponent<TrainController>();
            newTrainController.NextTrain = lastTrain;
            Trains.Add(newTrainController);
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

            _gameDataService.Score = TargetRail.Row - MapGenerator.Instance.DeltaRow;
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
                newSpeed * Time.fixedDeltaTime);
        }
    }
}
