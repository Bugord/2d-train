using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extentions;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public float Speed;
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


    public List<Vector3> TargetPointList;

    private Rigidbody2D _rigidbody2D;
    public SpriteRenderer _spriteRenderer;
    private const float DistToChangeTarget = 0.63f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsHeadTrain)
            return;
        Destroy(col.gameObject);

        if (col.tag == "Point")
        {


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


            var trainToRemove = Trains.Last();
            Trains.Remove(trainToRemove);
            Destroy(trainToRemove.gameObject);
        }
    }

    void GenerateNewTrain()
    {
        if (!IsHeadTrain)
            return;

        var lastTrain = Trains.Last();
        var newTrainPos = lastTrain.transform.position - lastTrain.transform.up * 0.8f;
        var newTrain = Instantiate(TrainPrefab, newTrainPos, Quaternion.identity);
        var newTrainController = newTrain.GetComponent<TrainController>();
        newTrainController.TargetRail = TargetRail;
        newTrainController.IsHeadTrain = false;
        Trains.First(controller => controller.IsHeadTrain).Trains.Add(newTrainController);
    }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(IsHeadTrain)
            Trains.Add(this);
    }

    void Start()
    {
        TargetPointList = TargetRail.CommonPoints.ToList();
        TargetPoint = TargetPointList[0];
    }

    void FixedUpdate()
    {
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
        _rigidbody2D.velocity = Speed * Vector3.Normalize(vectorToTarget);
    }

    private void ChangeTargetPoint()
    {
        TargetPointIndex++;
        if (TargetPointIndex >= TargetPointList.Count)
        {
            TargetPointIndex = 0;
            if (!Turning && !TargetRail.CurrentWayStruct.WayPoints.IsNullOrEmpty())
            {
                Turning = true;
                TargetPointList = TargetRail.CurrentWayStruct.WayPoints;
            }
            else
            {
                Turning = false;
                TargetRail = TargetRail.CurrentWayStruct.WayRailController;
                TargetPointList = TargetRail.CommonPoints;
            }
        }

        TargetPoint = TargetPointList[TargetPointIndex];
    }
}