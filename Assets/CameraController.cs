using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraController : MonoBehaviour
{
    public Transform Target;
    private float z;

    void OnEnable()
    {
        z = transform.position.z;
    }
	void Update ()
	{
	    transform.position = new Vector3(Target.position.x, Target.position.y, z);
	}
}
