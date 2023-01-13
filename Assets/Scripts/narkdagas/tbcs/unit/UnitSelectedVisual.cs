using System;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class UnitSelectedVisual : MonoBehaviour {
        private Unit _unit;
        private MeshRenderer _meshRenderer;
        private EventHandler _onSelectedUnitChanged;
        

        private void Awake() {
            _unit = GetComponentInParent<Unit>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start() {
            _onSelectedUnitChanged = delegate { UpdateVisual(); }; 
            UnitActionSystem.Instance.OnSelectedUnitChanged += _onSelectedUnitChanged;
            UpdateVisual();
        }

        private void UpdateVisual() {
            _meshRenderer.enabled = UnitActionSystem.Instance.GetSelectedUnit() == _unit;
        }

        private void OnDestroy() {
            UnitActionSystem.Instance.OnSelectedUnitChanged -= _onSelectedUnitChanged;
        }
    }
}