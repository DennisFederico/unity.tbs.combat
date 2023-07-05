using System.Collections.Generic;
using narkdagas.camera;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class FloorVisibility : MonoBehaviour {

        [SerializeField] private bool dynamicVisibility;
        [SerializeField] private List<Renderer> renderersToIgnore;
        private Renderer[] _childRenderers;
        private int _floorNumber;
        private readonly float _heightOffset = 2f;

        private void Awake() {
            _childRenderers = GetComponentsInChildren<Renderer>(true);
        }

        private void Start() {
            _floorNumber = LevelGrid.Instance.GetFloor(transform.position);
            if (_floorNumber == 0 && dynamicVisibility) {
                Destroy(this);
            }
        }

        private void Update() {
            var cameraHeight = CameraController.Instance.GetCameraHeight();
            _floorNumber = dynamicVisibility ? LevelGrid.Instance.GetFloor(transform.position) : _floorNumber;
            bool showObject = _floorNumber == 0 || _floorNumber * LevelGrid.FloorHeight <= cameraHeight - _heightOffset;
            if (showObject) {
                Show();
            } else {
                Hide();
            }
        }

        private void Show() {
            //TODO use the material alpha instead of renderer.enabled
            foreach (var childRenderer in _childRenderers) {
                if (renderersToIgnore.Contains(childRenderer)) continue;
                childRenderer.enabled = true;
            }
        }
        
        private void Hide() {
            foreach (var childRenderer in _childRenderers) {
                if (renderersToIgnore.Contains(childRenderer)) continue;
                childRenderer.enabled = false;
            }
        }
    }
}