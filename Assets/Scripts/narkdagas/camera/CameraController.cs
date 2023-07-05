using Cinemachine;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.systems;
using UnityEngine;

namespace narkdagas.camera {
    public class CameraController : MonoBehaviour {
        
        public static CameraController Instance { get; private set; }
        
        [SerializeField] private float cameraMoveSpeed = 10f;
        [SerializeField] private float cameraRotationSpeed = 100f;
        [SerializeField] private float cameraZoomFactor = .25f;
        [SerializeField] private float cameraZoomSpeed = 5f;
        [SerializeField] private float minCameraZoomValue = 2f;
        [SerializeField] private float maxCameraZoomValue = 18f;
        [SerializeField] private float screenMoveMargin = 5f;
        [SerializeField] private CinemachineVirtualCamera vCamera;
        private CinemachineTransposer _vCameraTransposer;
        private Vector3 _targetFollowOffset;
        private Vector3 _mousePosition = Vector3.zero;
        private bool _mousePositionCaptured;
        private float _minX, _maxX, _minZ, _maxZ;

        private void Awake() {
            
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _vCameraTransposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
            //World Position Bounds
            var gridDimension = LevelGrid.Instance.GetGridDimension(0);
            var bottomLeftCorner = LevelGrid.Instance.GetGridWorldPosition(new GridPosition(0, 0, 0));
            _minX = bottomLeftCorner.x;
            _minZ = bottomLeftCorner.z;
            var topRightCorner = LevelGrid.Instance.GetGridWorldPosition(new GridPosition(gridDimension.Width -1, gridDimension.Length -1, 0));
            _maxX = topRightCorner.x;
            _maxZ = topRightCorner.z;
        }

        private void Start() {
            _targetFollowOffset = _vCameraTransposer.m_FollowOffset;
        }

        void Update() {
            if (!Application.isFocused) return;
            HandleKeyboardMove();
            HandleMouseMove();
            HandleKeyboardRotation();
            HandleMouseRotation();
            HandleKeyboardZoom();
            HandleMouseZoom();
        }

        private void HandleKeyboardMove() {
            var inputMoveDir = InputManager.Instance.GetCameraMove();
            Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
            Vector3 unclampedPosition = transform.position + moveVector * (cameraMoveSpeed * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(unclampedPosition.x, _minX, _maxX), 0, Mathf.Clamp(unclampedPosition.z, _minZ, _maxZ));
        }

        private void HandleMouseMove() {
            Vector2 moveDir = Vector2.zero;
            var mousePosition = InputManager.Instance.GetMouseScreenPosition();
            // Debug.Log($"Mouse {mousePosition} | {Screen.width}x{Screen.height} | {Screen.currentResolution}");
            if (mousePosition.x >= Screen.width - screenMoveMargin) {
                moveDir.x = +1;
            }

            if (mousePosition.x <= screenMoveMargin) {
                moveDir.x = -1;
            }

            if (mousePosition.y >= Screen.height - screenMoveMargin) {
                moveDir.y = +1;
            }

            if (mousePosition.y <= screenMoveMargin) {
                moveDir.y = -1;
            }

            Vector3 moveVector = transform.forward * moveDir.y + transform.right * moveDir.x;
            transform.position += moveVector * (cameraMoveSpeed * Time.deltaTime);
        }

        private void HandleKeyboardRotation() {
            var rotation = InputManager.Instance.GetCameraRotation();
            transform.Rotate(Vector3.up, rotation * cameraRotationSpeed * Time.deltaTime);
        }

        private void HandleMouseRotation() {
            float rotation = 0;
            if (InputManager.Instance.IsRightMouseButtonDown()) _mousePosition = InputManager.Instance.GetMouseScreenPosition();
            if (InputManager.Instance.IsRightMouseButtonPressed()) {
                rotation = InputManager.Instance.GetMouseScreenPosition().x - _mousePosition.x;
                _mousePosition = InputManager.Instance.GetMouseScreenPosition();
            }

            transform.Rotate(Vector3.up, rotation * cameraRotationSpeed * Time.deltaTime);
        }

        private void HandleKeyboardZoom() {
            var zoom = InputManager.Instance.GetCameraZoom();
            if (zoom != 0) _targetFollowOffset.y += zoom * cameraZoomFactor;
            //TODO Try to zoom over z after z goes over some threshold 
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minCameraZoomValue, maxCameraZoomValue);
            _vCameraTransposer.m_FollowOffset = Vector3.Lerp(_vCameraTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }

        private void HandleMouseZoom() {
            var zoom = InputManager.Instance.GetMouseScroll();
            if (zoom != 0) _targetFollowOffset.y -= zoom;

            //TODO Try to zoom over z after z goes over some threshold 
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, minCameraZoomValue, maxCameraZoomValue);
            _vCameraTransposer.m_FollowOffset = Vector3.Lerp(_vCameraTransposer.m_FollowOffset, _targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }
        
        public float GetCameraHeight() {
            return _vCameraTransposer.m_FollowOffset.y;
        }
    }
}