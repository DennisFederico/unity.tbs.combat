using UnityEngine;

namespace narkdagas.tbcs.systems {
    public class MouseWorld : MonoBehaviour {
        private static MouseWorld _instance;

        public LayerMask validClickMasks;

        private void Awake() {
            _instance = this;
        }

        void Update() {
            transform.position = MouseWorld.GetPosition();
        }

        public static Vector3 GetPosition() {
            GetClickDataForMask(out var hit, _instance.validClickMasks);
            return hit.point;
        }

        public static bool GetClickDataForMask(out RaycastHit hit, LayerMask hitMask) {
            var screenPointToRay = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            return Physics.Raycast(screenPointToRay, out hit, float.MaxValue, hitMask);
        }
    }
}