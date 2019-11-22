using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    [SerializeField] private Transform _storeButton;

    // Use this for initialization
    void Start()
    {
        var yOffset = target.position.y - Camera.main.ScreenToWorldPoint(_storeButton.position).y;
        transform.position += Vector3.up * yOffset;

        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - target.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
    }
}