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
    
        public List<TrainController> Trains;
        public Transform LookAtTarget;
    
        public TrainController NextTrain;
        public static TrainController HeadTrain;
        public float distanceBetweenTrains;
    
        public bool IsDead;
        public bool IsBoosted;
    
        public Vector3 LastTrainPos;
        private List<Vector3> _lastPoints;

        protected AchievementsService _achievementsService;
        protected AudioService _audioService;
        protected LevelService _levelService;
        protected AdsService _adsService;
        protected SkinService _skinService;
        protected GameDataService _gameDataService;
        
        private void Awake()
        {
            LastTrainPos = transform.position;
            _lastPoints = new List<Vector3>();
            _achievementsService = ServiceLocator.GetService<AchievementsService>();
            _audioService = ServiceLocator.GetService<AudioService>();
            _levelService = ServiceLocator.GetService<LevelService>();
            _adsService = ServiceLocator.GetService<AdsService>();
            _skinService = ServiceLocator.GetService<SkinService>();
            _gameDataService = ServiceLocator.GetService<GameDataService>();
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
}