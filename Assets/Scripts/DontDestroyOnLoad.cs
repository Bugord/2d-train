using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);

        var objects = GameObject.FindGameObjectsWithTag("DontDestroy");

        if (objects.Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
