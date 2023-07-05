using UnityEngine;

namespace narkdagas.tbcs.systems {
    public class MouseWorld : MonoBehaviour {
        private static MouseWorld _instance;

        public LayerMask validClickMasks;

        private void Awake() {
            _instance = this;
        }

        // void Update() {
        //     transform.position = MouseWorld.GetPosition();
        // }

        public static Vector3 GetPosition() {
            GetClickDataForMask(out var hit, _instance.validClickMasks);
            return hit.point;
        }

        //TODO USE A SINGLETON, KEEP A REFERENCE OF THE MAIN CAMERA
        public static bool GetClickDataForMask(out RaycastHit hit, LayerMask hitMask) {
            var screenPointToRay = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            return Physics.Raycast(screenPointToRay, out hit, float.MaxValue, hitMask);
        }
        
        public static Vector3 GetVisiblePosition() {
            var screenPointToRay = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            var hits = Physics.RaycastAll(screenPointToRay, float.MaxValue, _instance.validClickMasks);
            //Sort by distance
            System.Array.Sort(hits, (x, y) => Mathf.RoundToInt(x.distance - y.distance));
            foreach (var hit in hits) {
                if (hit.transform.TryGetComponent<Renderer>(out var renderer)) {
                    if (renderer.enabled) {
                        return hit.point;
                    }
                }
            }
            return Vector3.zero;
        }
    }
}