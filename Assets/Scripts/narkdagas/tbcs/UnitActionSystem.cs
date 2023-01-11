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
        private Unit _selectedUnit;
        private BaseAction _selectedAction;
        private bool _isActionRunning;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one UnitActionSystem in the scene! {transform} -{Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update() {
            if (_isActionRunning) return;
            if (TryHandleUnitSelection()) return;
            HandleSelectedAction();
        }

        private bool TryHandleUnitSelection() {
            if (Input.GetMouseButtonDown(0)) {
                if (MouseWorld.GetClickDataForMask(out var hit, unitLayerMask)) {
                    if (hit.transform.TryGetComponent<Unit>(out Unit unit)) {
                        SetSelectedUnit(unit);
                        return true;
                    }
                }
            }
            return false;
        }

        private void HandleSelectedAction() {
            if (Input.GetMouseButtonDown(0)) {
                var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                switch (_selectedAction) {
                    case MoveAction moveAction:
                        if (moveAction.IsValidActionGridPosition(mouseGridPosition)) {
                            SetActionRunning();
                            moveAction.Move(mouseGridPosition, ClearActionRunning);                            
                        }
                        break;
                    case SpinAction spinAction:
                        SetActionRunning();
                        spinAction.Spin(ClearActionRunning);
                        break;
                }
            }
        }

        private void SetActionRunning() {
            _isActionRunning = true;
        }
        
        private void ClearActionRunning() {
            _isActionRunning = false;
        }

        private void SetSelectedUnit(Unit unit) {
            _selectedUnit = unit;
            SetSelectedAction(_selectedUnit.GetMoveAction());
            OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedAction(BaseAction baseAction) {
            _selectedAction = baseAction;
        }
        
        public Unit GetSelectedUnit() {
            return _selectedUnit;
        }
    }
}