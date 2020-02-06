using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float _smoothStep = 0.125f;
    private Vector3 offset;         
    [SerializeField] private RectTransform _storeButton;
    [SerializeField] private Transform _initialRail;
    
    // Use this for initialization
    void Start()
    {
        var storeButtonPosition = Camera.main.ScreenToWorldPoint(_storeButton.position) + Vector3.up * 0.07f;
        
        storeButtonPosition.z = 0;
        target.position = storeButtonPosition;
        _initialRail.position = storeButtonPosition;
        
        offset = transform.position - target.position;
    }
    
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothStep);
        transform.position = smoothedPosition;
    }
}