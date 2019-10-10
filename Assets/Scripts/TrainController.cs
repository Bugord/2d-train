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

    public List<Vector3> TargetPointList;

    private Rigidbody2D _rigidbody2D;
    private const float DistToChangeTarget = 0.63f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        TargetPointList = TargetRail.WayPoints;
        TargetPoint = TargetPointList[0];
    }

    private void Update()
    {
        var touch = Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (Input.GetKeyDown(KeyCode.Mouse0) || touch)
        {
            TargetRail.SwitchRail();
        }
    }

    private void FixedUpdate()
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
        TargetPoint = TargetPointList[TargetPointIndex];
    }
}