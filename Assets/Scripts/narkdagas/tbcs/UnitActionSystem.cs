using System;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs {
    public class UnitActionSystem : MonoBehaviour {

        public static UnitActionSystem Instance { get; private set; }
        public LayerMask validActionMasks;
        public LayerMask unitLayerMask;
        public LayerMask floorLayerMask;
        public event EventHandler OnSelectedUnitChange;
        [SerializeField] private Unit selectedUnit;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one UnitActionSystem in the scene! {transform} -{Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update() {

            if (Input.GetMouseButtonDown(0)) {
                if (MouseWorld.GetClickDataForMask(out var hit, validActionMasks)) {
                    //Handle based on the mask clicked
                    if (1 << hit.collider.gameObject.layer == unitLayerMask) {
                        HandleUnitSelected(hit.collider.gameObject.GetComponent<Unit>());
                        return;
                    }
                    if (1 << hit.collider.gameObject.layer == floorLayerMask) {
                        var mouseGridPosition = LevelGrid.Instance.GetGridPosition(hit.point);
                        if (selectedUnit.GetMoveAction().IsValidActionGridPosition(mouseGridPosition)) {
                            selectedUnit.GetMoveAction().Move(mouseGridPosition);    
                        }
                        return;
                    }
                }
            }

            if (Input.GetMouseButtonDown(1)) {
                selectedUnit.GetSpinAction().ToggleSpin();
            }
        }

        private void HandleUnitSelected(Unit unit) {
            selectedUnit = unit;
            OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
        }

        public Unit GetSelectedUnit() {
            return selectedUnit;
        }
    }
}