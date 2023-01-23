using System;
using System.Collections.Generic;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.systems;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class LevelGrid : MonoBehaviour {
        public static LevelGrid Instance { get; private set; }
        public event EventHandler OnAnyUnitMovedGridPosition;
        [SerializeField] private bool debugGrid;
        [SerializeField] private bool debugMousePosition;
        [SerializeField] private Transform debugPrefab;
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridLength;
        [SerializeField] private int gridCellSize;
        private GridSystemHex<GridObject> _gridSystemHex;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one LevelGrid in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _gridSystemHex = new GridSystemHex<GridObject>(gridWidth, gridLength, gridCellSize, GridObject.CtorFunction, debugGrid, debugPrefab);
        }

        void Update() {
            if (debugMousePosition) {
                var worldPos = MouseWorld.GetPosition();
                Debug.Log($"pos:{worldPos} | Grid:{GetGridPosition(worldPos)} | center:{_gridSystemHex.GetWorldPosition(_gridSystemHex.GetGridPosition(worldPos))}");
            }

            if (debugGrid) { }
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemHex.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemHex.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemHex.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to) {
            RemoveUnitAtGridPosition(from, unit);
            AddUnitAtGridPosition(to, unit);
            OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) {
            var gridObject = _gridSystemHex.GetGridObject(gridPosition);
            gridObject.Interactable = interactable;
        }

        public IInteractable GetDoorAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemHex.GetGridObject(gridPosition);
            return gridObject.Interactable;
        }

        //PASS THROUGH TO GRID SYSTEM
        public GridDimension GetGridDimension() => _gridSystemHex.GetGridDimension();
        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystemHex.GetGridPosition(worldPos);
        public Vector3 GetGridWorldPosition(GridPosition gridPosition) => _gridSystemHex.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystemHex.IsValidGridPosition(gridPosition);
        public bool IsGridPositionFree(GridPosition gridPosition) => !_gridSystemHex.GetGridObject(gridPosition).HasAnyUnit();
        public Unit GetUnitAtGridPosition(GridPosition gridPosition) => _gridSystemHex.GetGridObject(gridPosition).GetUnit();
        public bool IsEnemyAtGridPosition(GridPosition gridPosition, bool isEnemy) => _gridSystemHex.GetGridObject(gridPosition).ContainsEnemy(isEnemy);
        public bool IsInteractableAtGridPosition(GridPosition gridPosition) => _gridSystemHex.GetGridObject(gridPosition).Interactable != null;
    }
}