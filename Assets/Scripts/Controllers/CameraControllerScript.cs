using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;
    
    private Vector3 offset;         
    [SerializeField] private Transform _storeButton;
    
    // Use this for initialization
    void Start()
    {
        var storeButtonPosition = Camera.main.ScreenToWorldPoint(_storeButton.position);
        
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        //offset = transform.position - new Vector3(storeButtonPosition.x, storeButtonPosition.y, 0) - Vector3.up*0.07f;
        offset = transform.position - target.position;
    }
    
    void Update()
    {
        transform.position = target.position + offset;
    }
}