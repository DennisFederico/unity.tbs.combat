using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class LookAtCameraUI : MonoBehaviour {

        [SerializeField] private bool invert;
        private Transform _cameraTransform;

        private void Awake() {
            _cameraTransform = Camera.main.transform;
        }

        private void LateUpdate() {
            var position = transform.position;
            var direction = (_cameraTransform.position - position).normalized;
            transform.LookAt(invert? position + direction * -1: position + direction);
        }
    }
}