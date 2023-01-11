using UnityEngine;

namespace narkdagas.tbcs {
    public class UnitSelectedVisual : MonoBehaviour {
        private Unit _unit;
        private MeshRenderer _meshRenderer;

        private void Awake() {
            _unit = GetComponentInParent<Unit>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start() {
            //UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedUnitChanged += (_, _) => {
                UpdateVisual();
            };
            UpdateVisual();
        }

        private void UpdateVisual () {
            _meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == _unit;
        }
    }
}