using System;
using System.Collections.Generic;
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
        [SerializeField] private int gridLenght;
        [SerializeField] private int gridCellSize;
        private GridSystem _gridSystem;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one LevelGrid in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _gridSystem = new GridSystem(gridWidth, gridLenght, gridCellSize, debugGrid, debugPrefab);
        }

        void Update() {
            if (debugMousePosition) {
                var worldPos = MouseWorld.GetPosition();
                Debug.Log($"pos:{worldPos} | Grid:{GetGridPosition(worldPos)} | center:{_gridSystem.GetWorldPosition(_gridSystem.GetGridPosition(worldPos))}");
            }

            if (debugGrid) { }
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to) {
            RemoveUnitAtGridPosition(from, unit);
            AddUnitAtGridPosition(to, unit);
            OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }

        //PASS THROUGH TO GRID SYSTEM
        public GridDimension GetGridDimension() => _gridSystem.GetGridDimension();
        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystem.GetGridPosition(worldPos);
        public Vector3 GetGridWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);
        public bool IsGridPositionFree(GridPosition gridPosition) => !_gridSystem.GetGridObject(gridPosition).HasAnyUnit();
        public Unit GetUnitAtGridPosition(GridPosition gridPosition) => _gridSystem.GetGridObject(gridPosition).GetUnit();
        public bool IsEnemyAtGridPosition(GridPosition gridPosition, bool isEnemy) => _gridSystem.GetGridObject(gridPosition).ContainsEnemy(isEnemy);

        //SHOULD WE HANDLE THE INVOCATIONS TO THE VISUAL SYSTEM TOO?
    }
}