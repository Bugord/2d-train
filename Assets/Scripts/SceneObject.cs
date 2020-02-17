using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ObjectsPool;
using UnityEngine;

public class SceneObject : PoolObject
{
    private Camera _camera;

    void Awake()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (_camera.transform.position.y > transform.position.y + 10)
        {
            ReturnToPool();
        }
    }
}
