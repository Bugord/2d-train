using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float _smoothStep = 0.125f;
    private Vector3 offset;         
    [SerializeField] private RectTransform _storeButton;
    [SerializeField] private Transform _initialRail;
    private Camera _camera;

    private float H;
    private float S;
    private float V;
    
    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
        var storeButtonPosition = _camera.ScreenToWorldPoint(_storeButton.position) + Vector3.up*0.15f;
        
        storeButtonPosition.z = 0;
        target.position = storeButtonPosition;
        _initialRail.position = storeButtonPosition;
        
        offset = transform.position - target.position;
        
        Color.RGBToHSV(_camera.backgroundColor,out H,out S,out V);
        H *= 360;
    }
    
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothStep);
        transform.position = smoothedPosition;
    }

    void LateUpdate()
    {
        if (H >= 360)
        {
            H = 0;
        }
        
        H += Time.deltaTime*2;
        
        _camera.backgroundColor = Color.HSVToRGB(H / 360, S, V);
    }
}