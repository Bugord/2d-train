using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainController : MonoBehaviour
{
    public float Speed;
    public float RotationSpeed;
    public Vector3 target;

    public bool turned;
    public int RailRow;
    public int RailIndex;
    public int way;
    public RailController RailController;

    public Vector3[] CurrentWay;
    public int CurrentPoint;

    private Rigidbody2D rigidbody2D;

    void OnEnable()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        CurrentWay = RailController.CommonPoints;
        target = CurrentWay[0];
    }

    void FixedUpdate()
    {
        Vector3 vectorToTarget = target - transform.position + RailController.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * RotationSpeed);

        rigidbody2D.velocity = Speed * Vector3.Normalize(target + RailController.transform.position - transform.position);

        if (Vector3.Distance(transform.position, target  + RailController.transform.position) < 0.4f)
        {
            CurrentPoint++;
            if (CurrentPoint >= CurrentWay.Length)
            {
                if (!turned)
                {
                    turned = true;
                    way = RailController.TypeRail;
                    CurrentWay = RailController.PointLists[way].Points;
                    CurrentPoint = 0;
                }
                else
                {
                    turned = false;
                    RailController = RailController.NextRailControllers[way];
                    RailRow = RailController.Row;
                    RailIndex = RailController.index;
                    way = RailController.TypeRail;
                    CurrentWay = RailController.CommonPoints;
                    CurrentPoint = 0;
                }
            }
            target = CurrentWay[CurrentPoint];
        }
    }
}
