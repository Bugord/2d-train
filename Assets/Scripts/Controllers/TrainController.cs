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
        
        public Transform LookAtTarget;
    
        public TrainController NextTrain;
        public float distanceBetweenTrains;
    
        public bool IsDead;
        public bool IsBoosted;
    
        public Vector3 LastTrainPos;
        private List<Vector3> _lastPoints = new List<Vector3>();
        
        private void FixedUpdate()
        {
            SetLastTrainPos();
            
            SetRotation(NextTrain.LookAtTarget.position - transform.position);
            MoveTrain(NextTrain.LastTrainPos);
        }

        public void SetLastTrainPos()
        {
            _lastPoints.Add(transform.position);
            _lastPoints.RemoveAll(p => Vector3.Distance(transform.position, p) >= distanceBetweenTrains);
            LastTrainPos = _lastPoints.First();
        }
    
        public void SetRotation(Vector2 vectorToTarget)
        {
            transform.up = Vector3.Lerp(transform.up, vectorToTarget, 0.5f);
        }

        public virtual void MoveTrain(Vector2 vectorToTarget)
        {
            if (IsDead) return;
        
            transform.position = Vector3.Lerp(transform.position, NextTrain.LastTrainPos, 0.85f);
        }
    
    
    }
}