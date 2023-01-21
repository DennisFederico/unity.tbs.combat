using UnityEngine;

namespace narkdagas.tbcs.systems {
    public class InputManager : MonoBehaviour {

        public static InputManager Instance { get; private set; }
        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one InputManager in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        
        public Vector2 GetMouseScreenPosition() {
            return Input.mousePosition;
        }

        public bool IsLeftMouseButtonDown() {
            return Input.GetMouseButtonDown(0);
        }
        
        public bool IsLeftMouseButtonPressed() {
            return Input.GetMouseButton(0);
        }
        
        public bool IsRightMouseButtonDown() {
            return Input.GetMouseButtonDown(1);
        }
        
        public bool IsRightMouseButtonPressed() {
            return Input.GetMouseButton(1);
        }
        
        public Vector2 GetMouseScroll() {
            return Input.mouseScrollDelta;
        }

        public Vector2 GetCameraMove() {
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
        }

        public float GetCameraRotation() {
            float rotation = 0;
            if (Input.GetKey(KeyCode.Q)) {
                rotation = +1;
            }

            if (Input.GetKey(KeyCode.E)) {
                rotation = -1;
            }

            return rotation;
        }

        public float GetCameraZoom() {
            float zoom = 0;
            if (Input.GetKey(KeyCode.R)) {
                zoom = -1;
            }

            if (Input.GetKey(KeyCode.F)) {
                zoom = +1;
            }

            return zoom;
        }
        
    }
}