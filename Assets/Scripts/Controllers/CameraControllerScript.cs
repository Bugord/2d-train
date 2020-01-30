using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;
    
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
    
    void Update()
    {
        transform.position = target.position + offset;
    }
}