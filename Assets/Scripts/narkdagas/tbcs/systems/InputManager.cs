#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace narkdagas.tbcs.systems {
    public class InputManager : MonoBehaviour {
        public static InputManager Instance { get; private set; }
        private CameraControlInputAction _cameraControlInputAction;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one InputManager in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
#if USE_NEW_INPUT_SYSTEM
            _cameraControlInputAction = new CameraControlInputAction();
            _cameraControlInputAction.Keyboard.Enable();
#endif
        }

        public Vector2 GetMouseScreenPosition() {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        public bool IsLeftMouseButtonDown() {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.leftButton.wasPressedThisFrame;
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public bool IsLeftMouseButtonPressed() {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.leftButton.isPressed;
#else
            return Input.GetMouseButton(0);
#endif
        }

        public bool IsRightMouseButtonDown() {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.rightButton.wasPressedThisFrame;
#else
            return Input.GetMouseButtonDown(1);
#endif
        }

        public bool IsRightMouseButtonPressed() {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.rightButton.isPressed;
#else
            return Input.GetMouseButton(1);
#endif
        }

        public float GetMouseScroll() {
#if USE_NEW_INPUT_SYSTEM
            return _cameraControlInputAction.Keyboard.MouseZoom.ReadValue<float>();
#else
            return Input.mouseScrollDelta.y;
#endif
        }

        public Vector2 GetCameraMove() {
#if USE_NEW_INPUT_SYSTEM
            return _cameraControlInputAction.Keyboard.CameraMovement.ReadValue<Vector2>();
#else
            Vector2 inputMoveDir = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) {
                inputMoveDir.y = +1;
            }

            if (Input.GetKey(KeyCode.S)) {
                inputMoveDir.y = -1;
            }

            if (Input.GetKey(KeyCode.D)) {
                inputMoveDir.x = +1;
            }

            if (Input.GetKey(KeyCode.A)) {
                inputMoveDir.x = -1;
            }
            return inputMoveDir;
#endif
        }

        public float GetCameraRotation() {
#if USE_NEW_INPUT_SYSTEM
            return _cameraControlInputAction.Keyboard.CameraRotate.ReadValue<float>();
#else
            float rotation = 0;
            if (Input.GetKey(KeyCode.Q)) {
                rotation = +1;
            }

            if (Input.GetKey(KeyCode.E)) {
                rotation = -1;
            }
            return rotation;
#endif
        }

        public float GetCameraZoom() {
#if USE_NEW_INPUT_SYSTEM
            return _cameraControlInputAction.Keyboard.CameraZoom.ReadValue<float>();
#else
            float zoom = 0;
            if (Input.GetKey(KeyCode.R)) {
                zoom = -1;
            }

            if (Input.GetKey(KeyCode.F)) {
                zoom = +1;
            }
            return zoom;
#endif
        }
    }
}