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
        [SerializeField] private float screenMoveMargin = 5f;
        [SerializeField] private CinemachineVirtualCamera vCamera;
        private CinemachineTransposer _vCameraTransposer;
        private Vector3 _targetFollowOffset;
        private Vector3 _mousePosition = Vector3.zero;
        private bool _mousePositionCaptured;

        private void Awake() {
            _vCameraTransposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void Start() {
            _targetFollowOffset = _vCameraTransposer.m_FollowOffset;
        }

        void Update() {
            HandleKeyboardMove();
            HandleMouseMove();
            HandleKeyboardRotation();
            HandleMouseRotation();
            HandleKeyboardZoom();
            HandleMouseZoom();
        }

        private void HandleKeyboardMove() {
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
        }

        private void HandleMouseMove() {
            Vector3 moveDir = Vector3.zero;
            var mousePosition = Input.mousePosition;
            // Debug.Log($"Mouse {mousePosition} | {Screen.width}x{Screen.height} | {Screen.currentResolution}");
            if (mousePosition.x >= Screen.width - screenMoveMargin) {
                moveDir.x = +1;
            }
            if (mousePosition.x <= screenMoveMargin) {
                moveDir.x = -1;
            }
            if (mousePosition.y >= Screen.height - screenMoveMargin) {
                moveDir.z = +1;
            }
            if (mousePosition.y <= screenMoveMargin) {
                moveDir.z = -1;
            }
            Vector3 moveVector = transform.forward * moveDir.z + transform.right * moveDir.x;
            transform.position += moveVector * (cameraMoveSpeed * Time.deltaTime);
        }

        private void HandleKeyboardRotation() {
            float rotation = 0;
            if (Input.GetKey(KeyCode.Q)) {
                rotation = +1;
            }

            if (Input.GetKey(KeyCode.E)) {
                rotation = -1;
            }
            transform.Rotate(Vector3.up, rotation * cameraRotationSpeed * Time.deltaTime);
        }

        private void HandleMouseRotation() {
            float rotation = 0;
            if (Input.GetMouseButtonDown(1)) _mousePosition = Input.mousePosition;
            if (Input.GetMouseButton(1)) {
                rotation = Input.mousePosition.x - _mousePosition.x;
                _mousePosition = Input.mousePosition;
            }
            transform.Rotate(Vector3.up, rotation * cameraRotationSpeed * Time.deltaTime);
        }
        
        private void HandleKeyboardZoom() {
            float zoom = 0;
            if (Input.GetKey(KeyCode.R)) {
                zoom = -1;
            }

            if (Input.GetKey(KeyCode.F)) {
                zoom = +1;
            }

            if (zoom != 0) _targetFollowOffset.y += zoom * cameraZoomFactor;

            //TODO Try to zoom over z after z goes over some threshold 
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minCameraZoomValue, maxCameraZoomValue);
            _vCameraTransposer.m_FollowOffset = Vector3.Lerp(_vCameraTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }

        private void HandleMouseZoom() {
            Vector2 mouseScroll = Input.mouseScrollDelta;
            if (mouseScroll.y != 0) _targetFollowOffset.y -= mouseScroll.y;

            //TODO Try to zoom over z after z goes over some threshold 
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minCameraZoomValue, maxCameraZoomValue);
            _vCameraTransposer.m_FollowOffset = Vector3.Lerp(_vCameraTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }
    }
}