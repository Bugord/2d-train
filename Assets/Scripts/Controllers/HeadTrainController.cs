using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Extentions;
using Assets.Scripts.ObjectsPool;
using Assets.Scripts.Services;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Animator _boostAnimator;
        
        private AchievementsService _achievementsService;
        private AudioService _audioService;
        private LevelService _levelService;
        private AdsService _adsService;
        private SkinService _skinService;
        private GameDataService _gameDataService;
        private UIService _uiService;
        private PoolService _poolService;
        
        private bool LockControlls;
        private bool IsFirstTime;

        private int _starsInOneRun = 0;
        private int _stopsSurvived = 0;

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
            _poolService = ServiceLocator.GetService<PoolService>();

            Trains = new List<TrainController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            TargetPointList = TargetRail.WayPoints;
            TargetPoint = TargetPointList[0].localPosition;
            Trains.Add(this);

            InputManager.Swipe += InputManagerOnSwipe;
            _adsService.TrainRevive += ReviveTrain;
            _uiService.GameRestart += ResetTrain;

            _skinService.UpdateSkin(spriteRenderer);
            UpdateTrainPoints();
            LockControlls = _uiService.IsFirstTime;
            IsFirstTime = _uiService.IsFirstTime;
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
            if (!_uiService.IsInGame) return;
            
            var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;

            if ((Input.GetKeyDown(KeyCode.Mouse0) || touch) && !LockControlls)
            {
                TargetRail.SwitchRail();
                _audioService.Play(AudioClipType.Swipe);

                if (IsFirstTime)
                {
                    Row nextTutorRow = MapGenerator.Instance.TutorialRows.FirstOrDefault();
                    if (_uiService.IsInTutorial && nextTutorRow != null)
                    {
                        if (!nextTutorRow.Outputs.First(o => o.Key == TargetRail.NextActiveRail.OutputId).Value.HasObject)
                        {
                            _uiService.ShowTutorial(false);
                            MapGenerator.Instance.TutorialRows.Remove(nextTutorRow);
                            if (MapGenerator.Instance.TutorialRows.Count != 0)
                            {
                                LockControlls = true;
                            }
                            else
                            {
                                _uiService.IsFirstTime = false;
                                IsFirstTime = false;
                            }
                        }
                    }
                }
            }
        }
#endif

        private void FixedUpdate()
        {
            if (!_uiService.IsInGame || _uiService.IsInTutorial) return;
            
            if (IsFirstTime)
            {
                Row nextTutorRow = MapGenerator.Instance.TutorialRows.FirstOrDefault();
                if (nextTutorRow != null && MapGenerator._rowsList[TargetRail.NextActiveRail.Row] == nextTutorRow)
                {
                    if (Vector2.Distance(nextTutorRow.Rails.First().transform.position, transform.position) < 2)
                    {
                        SwipeDirection swipeDirection =
                            TargetRail.OutputId > nextTutorRow.Outputs.First(o => !o.Value.HasObject).Key
                                ? SwipeDirection.Left
                                : SwipeDirection.Right;
                        
                        _uiService.ShowTutorial(true, swipeDirection);
                        LockControlls = false;
                    }
                    else
                    {
                        LockControlls = true;
                    }
                }
            }

            var vectorToTarget = VectorToTarget();

            SetRotation(vectorToTarget);
            MoveTrain(vectorToTarget);

            _boostAnimator.transform.rotation = Quaternion.identity;
            
            if (Vector2.SqrMagnitude(vectorToTarget) < DistToChangeTarget)
            {
                ChangeTargetPoint();
            }
            _uiService.UpdateInGameDistance(_gameDataService.Score);
        }

        private void InputManagerOnSwipe(SwipeDirection direction)
        {
#if !UNITY_EDITOR
        if (!_uiService.IsInGame || LockControlls) return;
        TargetRail.SwitchRail(direction);
        _audioService.Play(AudioClipType.Swipe, direction);
        if (IsFirstTime)
        {
            Row nextTutorRow = MapGenerator.Instance.TutorialRows.FirstOrDefault();
            if (_uiService.IsInTutorial && nextTutorRow != null)
            {
                if (!nextTutorRow.Outputs.First(o => o.Key == TargetRail.NextActiveRail.OutputId).Value.HasObject)
                {
                    _uiService.ShowTutorial(false);
                    MapGenerator.Instance.TutorialRows.Remove(nextTutorRow);
                    if (MapGenerator.Instance.TutorialRows.Count != 0)
                    {
                        LockControlls = true;
                    }
                    else
                    {
                        _uiService.IsFirstTime = false;
                        IsFirstTime = false;
                    }
                }
            }
        }
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
                _achievementsService.UnlockAchievement(GPGSIds.achievement_coin_of_luck);
                if (_gameDataService.Coins == 100)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_small_collection);
                } else if (_gameDataService.Coins == 500)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_numismatist);
                } else if (_gameDataService.Coins == 1000)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_good_catch);
                }

                _audioService.Play(AudioClipType.Coin);
            }
            else if (col.CompareTag("Boost"))
            {
                poolObject.ReturnToPool();
                StartCoroutine(ActivateBoost());
                _achievementsService.UnlockAchievement(GPGSIds.achievement_lone_star);
                _starsInOneRun++;
                if (_starsInOneRun == 3)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_a_pinch_of_stardust);
                } else if (_starsInOneRun == 5)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_starlight);
                } else if (_starsInOneRun == 10)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_speed_of_light);
                }
                _achievementsService.IncrementAchievement(GPGSIds.achievement_constellation, 1);
                _achievementsService.IncrementAchievement(GPGSIds.achievement_star_cluster, 1);
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
                    trainToRemove.ReturnToPool(1.5f);
                }
                else
                {
                    _uiService.IsInGame = false;
                    _uiService.ShowReviveMenu(!_gameDataService.Revived);
                }
                _audioService.Play(AudioClipType.StopHit);
                _stopsSurvived++;
                if (_stopsSurvived == 30)
                {
                    _achievementsService.UnlockAchievement(GPGSIds.achievement_just_a_scratch);
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
                
                train.ReturnToPool();
            });

            Trains.Clear();
            Trains.Add(this);

            IsBoosted = false;
            _trailObject.SetActive(false);
            _starsInOneRun = 0;
            _stopsSurvived = 0;

            GameObject.FindGameObjectsWithTag("Stop").ToList().Where(s => Vector3.Distance(s.transform.position, transform.position) < 20).ToList().
                ForEach(stop => stop.GetComponent<PoolObject>().ReturnToPool());
            GameObject.FindGameObjectsWithTag("Boost").ToList().ForEach(boost => boost.GetComponent<PoolObject>().ReturnToPool());
        }

        private IEnumerator ActivateBoost()
        {
            _audioService.Play(AudioClipType.BoostStart);
            _boostAnimator.gameObject.SetActive(true);
            _boostAnimator.Play("Boost");
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
            _boostAnimator.gameObject.SetActive(false);
            _trailObject.SetActive(false);
            _pointEffector.SetActive(false);
            _particleSystem.SetActive(false);
        }

        private void GenerateNewTrain()
        {
            var lastTrain = Trains.Last();
            var newTrainPos = lastTrain.LastTrainPos;
            var newTrainController = _poolService.GetObject<TrainController>("Train", newTrainPos);
            newTrainController.NextTrain = lastTrain;
            newTrainController.IsDead = false;
            newTrainController.StartCoroutine(newTrainController.SetLastTrainPos());
            Trains.Add(newTrainController);
            newTrainController.spriteRenderer.sprite = spriteRenderer.sprite;
            _audioService.Play(AudioClipType.NewTrain);

            if (Trains.Count == 15)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_long_long_train);
            }
            _achievementsService.IncrementAchievement(GPGSIds.achievement_little_factory, 1);
            _achievementsService.IncrementAchievement(GPGSIds.achievement_train_company, 1);
            _achievementsService.IncrementAchievement(GPGSIds.achievement_steam_baron, 1);
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

            if (_gameDataService.Score == 100)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_first_steps);
            } else if (_gameDataService.Score == 250)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_next_station);
            } else  if (_gameDataService.Score == 500)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_on_a_rail);
            } else  if (_gameDataService.Score == 1000)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_long_journey);
            } else  if (_gameDataService.Score == 2000)
            {
                _achievementsService.UnlockAchievement(GPGSIds.achievement_transcontinental_railway);
            }
        }
        
        public override void MoveTrain(Vector2 vectorToTarget)
        {
            if (IsDead) return;

            speed = _levelService.GetSpeed();

            if (IsBoosted)
            {
                speed = _levelService.BoostedSpeed;
            }

            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + vectorToTarget,
                speed * Time.fixedDeltaTime);
        }
    }
}
