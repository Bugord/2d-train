using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ObjectsPool;
using UnityEngine;

public class SceneObject : PoolObject
{
    void Update()
    {
        if (Camera.main.transform.position.y > transform.position.y + 10)
        {
            ReturnToPool();
        }
    }
}
