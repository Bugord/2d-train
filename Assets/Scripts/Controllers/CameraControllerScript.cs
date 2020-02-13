using UnityEngine;

namespace Controllers
{
    public class CameraControllerScript : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private float _smoothStepY = 0.125f;
        [SerializeField] private float _smoothStepX = 0.1f;
        
        private Vector3 offset;         
        [SerializeField] private RectTransform _storeButton;
        [SerializeField] private Transform _initialRail;
        private Camera _camera;

        [SerializeField] private Gradient _gradient;

        [SerializeField] private float _gradientSmoothness;
        
        // Use this for initialization
        void Start()
        {
            _camera = GetComponent<Camera>();
            var storeButtonPosition = _storeButton.position + Vector3.up*0.15f;
            
            storeButtonPosition.z = 0;
            target.position = storeButtonPosition;
            _initialRail.position = storeButtonPosition;
        
            offset = transform.position - target.position;
        }
    
        void FixedUpdate()
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 position = transform.position;
            float smoothedPositionX = Mathf.Lerp(position.x, desiredPosition.x, _smoothStepX);
            float smoothedPositionY = Mathf.Lerp(position.y, desiredPosition.y, _smoothStepY);
            transform.position = new Vector3(smoothedPositionX, smoothedPositionY, position.z);
        }
    
        void Update()
        {
            _camera.backgroundColor = _gradient.Evaluate(Mathf.PingPong(Time.time/_gradientSmoothness, 1));
        }
    }
}