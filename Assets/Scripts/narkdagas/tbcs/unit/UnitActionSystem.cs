using System;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace narkdagas.tbcs.unit {
    public class UnitActionSystem : MonoBehaviour {
        public static UnitActionSystem Instance { get; private set; }
        public LayerMask unitLayerMask;
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnActionRunningChanged;
        private Unit _selectedUnit;
        private BaseAction _selectedAction;
        private bool _isActionRunning;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one UnitActionSystem in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            TurnSystem.Instance.OnTurnChanged += (_, isPlayerTurn) => {
                if (!isPlayerTurn) {
                    ClearSelectedUnit();
                }
            };
        }

        private void Update() {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!TurnSystem.Instance.IsPlayerTurn()) return;
            //I LIKE TO BE ABLE TO SWITCH UNITS WHILE THE OTHER UNIT IS IN AN ACTION
            //SWAP THE NEXT TWO LINES OTHERWISE
            //TODO IT WOULD BE GREAT FOR EACH UNIT TO HAVE THEIR "RUNNING ACTION" FLAG AND ACT IN PARALLEL
            if (TryHandleUnitSelection()) return;
            if (_isActionRunning) return;
            HandleSelectedAction();
        }

        private bool TryHandleUnitSelection() {
            if (Input.GetMouseButtonDown(0)) {
                if (MouseWorld.GetClickDataForMask(out var hit, unitLayerMask)) {
                    if (hit.transform.TryGetComponent(out Unit unit)) {
                        //DONT SELECT THE UNIT IF IT IS ALREADY SELECTED
                        if (unit != _selectedUnit && !unit.IsEnemyUnit()) {
                            SetSelectedUnit(unit);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void HandleSelectedAction() {
            if (Input.GetMouseButtonDown(0) && _selectedAction) {
                var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
                if (!_selectedUnit.TrySpendActionPoints(_selectedAction)) return;
                SetActionRunning();
                _selectedAction.TakeAction(mouseGridPosition, ClearActionRunning);
            }
        }

        private void SetActionRunning() {
            _isActionRunning = true;
            OnActionRunningChanged?.Invoke(this, _isActionRunning);
            //UnitActionSystemUI.Instance.SetShowActionsVisual(false);
            //UnitActionSystemUI.Instance.ToggleShowActionsVisual();
        }

        private void ClearActionRunning() {
            _isActionRunning = false;
            OnActionRunningChanged?.Invoke(this, _isActionRunning);
            //UnitActionSystemUI.Instance.SetShowActionsVisual(true);
            //UnitActionSystemUI.Instance.ToggleShowActionsVisual();
        }

        public Unit GetSelectedUnit() {
            return _selectedUnit;
        }

        private void SetSelectedUnit(Unit unit) {
            _selectedUnit = unit;
            SetSelectedAction(_selectedUnit.GetMoveAction());
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSelectedUnit() {
            ClearSelectedAction();
            _selectedUnit = null;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public BaseAction GetSelectedAction() {
            return _selectedAction;
        }

        public void SetSelectedAction(BaseAction baseAction) {
            _selectedAction = baseAction;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSelectedAction() {
            _selectedAction = null;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}