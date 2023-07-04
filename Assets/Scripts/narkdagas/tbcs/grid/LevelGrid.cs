using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private int numFloors;
        public const float FloorHeight = 3f; 
        private readonly List<GridSystemHex<GridObject>> _gridSystemHex = new();

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one LevelGrid in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            foreach (var floor in Enumerable.Range(0, numFloors)) {
                var gridLevel = new GridSystemHex<GridObject>(floor, gridWidth, gridLength, gridCellSize, FloorHeight, GridObject.CtorFunction, debugGrid, debugPrefab);
                _gridSystemHex.Add(gridLevel);
            }
        }

        void Update() {
            // if (debugMousePosition) {
            //     var worldPos = MouseWorld.GetPosition();
            //     //Debug.Log($"pos:{worldPos} | Grid:{GetGridPosition(worldPos)} | center:{_gridSystemHex.GetWorldPosition(_gridSystemHex.GetGridPosition(worldPos))}");
            //     Debug.Log($"worldPosition: {worldPosition} - Floor: {Mathf.FloorToInt(worldPosition.y / FloorHeight)}");
            // }

            // if (debugGrid) { }
        }
        
        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to) {
            RemoveUnitAtGridPosition(from, unit);
            AddUnitAtGridPosition(to, unit);
            OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) {
            var gridObject = _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.Interactable = interactable;
        }

        public IInteractable GetDoorAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition);
            return gridObject.Interactable;
        }
        
        private GridSystemHex<GridObject> GetGridSystem(int floor) {
            return _gridSystemHex[floor];
        }

        public int GetFloor(Vector3 worldPosition) {
            // Debug.Log($"worldPosition: {worldPosition} - Floor: {Mathf.FloorToInt(worldPosition.y / FloorHeight)}");
            return worldPosition.y <= 0 ? 0 : Mathf.FloorToInt(worldPosition.y / FloorHeight);
        }

        //PASS THROUGH TO GRID SYSTEM
        public GridDimension GetGridDimension(int floorNumber) => _gridSystemHex[floorNumber].GetGridDimension();
        public int GetNumberOfFloors() => numFloors;
        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystemHex[GetFloor(worldPos)].GetGridPosition(worldPos);
        public Vector3 GetGridWorldPosition(GridPosition gridPosition) => _gridSystemHex[gridPosition.FloorNumber].GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) {
            if (gridPosition.FloorNumber < 0 || gridPosition.FloorNumber >= numFloors) return false;
            return _gridSystemHex[gridPosition.FloorNumber].IsValidGridPosition(gridPosition);
        }

        public bool IsGridPositionFree(GridPosition gridPosition) => !_gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition).HasAnyUnit();
        public Unit GetUnitAtGridPosition(GridPosition gridPosition) => _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition).GetUnit();
        public bool IsEnemyAtGridPosition(GridPosition gridPosition, bool isEnemy) => _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition).ContainsEnemy(isEnemy);
        public bool IsInteractableAtGridPosition(GridPosition gridPosition) => _gridSystemHex[gridPosition.FloorNumber].GetGridObject(gridPosition).Interactable != null;
    }
}