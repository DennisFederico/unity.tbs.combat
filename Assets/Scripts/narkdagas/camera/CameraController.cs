using Cinemachine;
using UnityEngine;

namespace narkdagas.camera {
    public class CameraController : MonoBehaviour {
        
        [SerializeField] private float cameraMoveSpeed = 10f;
        [SerializeField] private float cameraRotationSpeed = 100f;
        [SerializeField] private float cameraZoomFactor = .25f;
        [SerializeField] private float cameraZoomSpeed = 5f;
        [SerializeField] private float minCameraZoomValue = 2f;
        [SerializeField] private float maxCameraZoomValue = 14f;
        [SerializeField] private CinemachineVirtualCamera vCamera;
        private CinemachineTransposer _vCameraTransposer;
        private Vector3 _targetFollowOffset;

        private void Awake() {
            _vCameraTransposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void Start() {
            _targetFollowOffset = _vCameraTransposer.m_FollowOffset;
            Debug.Log($"Starting OFFSET:{_targetFollowOffset}");
        }

        void Update() {
            Vector3 inputMoveDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) {
                inputMoveDir.z = +1;
            }
            if (Input.GetKey(KeyCode.S)) {
                inputMoveDir.z = -1;
            }
            if (Input.GetKey(KeyCode.D)) {
                inputMoveDir.x = +1;
            }
            if (Input.GetKey(KeyCode.A)) {
                inputMoveDir.x = -1;
            }
            Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
            transform.position += moveVector * (cameraMoveSpeed * Time.deltaTime);

            float rotation = 0;
            if (Input.GetKey(KeyCode.Q)) {
                rotation = +1;
            }
            if (Input.GetKey(KeyCode.E)) {
                rotation = -1;
            }
            transform.Rotate(Vector3.up, rotation * cameraRotationSpeed * Time.deltaTime);
            
            float zoom = 0;
            if (Input.GetKey(KeyCode.R)) {
                zoom = -1;
            }
            if (Input.GetKey(KeyCode.F)) {
                zoom = +1;
            }
            if (zoom != 0) _targetFollowOffset.y += zoom * cameraZoomFactor;
            
            Vector2 mouseScroll = Input.mouseScrollDelta;
            if (mouseScroll.y != 0) _targetFollowOffset.y -= mouseScroll.y;

            //TODO Try to zoom over z after z goes over some threshold 
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minCameraZoomValue, maxCameraZoomValue);
            _vCameraTransposer.m_FollowOffset = Vector3.Lerp(_vCameraTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }
    }
}