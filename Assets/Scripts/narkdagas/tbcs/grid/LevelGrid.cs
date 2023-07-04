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
        [SerializeField] private int gridLenght;
        [SerializeField] private int gridCellSize;
        [SerializeField] private int numFloors;
        public const float FloorHeight = 3f; 
        private readonly List<GridSystemSquare<GridObject>> _gridSystemSquare = new();

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one LevelGrid in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            foreach (var floor in Enumerable.Range(0, numFloors)) {
                var gridLevel = new GridSystemSquare<GridObject>(floor, gridWidth, gridLenght, gridCellSize, FloorHeight, GridObject.CtorFunction, debugGrid, debugPrefab);
                _gridSystemSquare.Add(gridLevel);
            }
            
        }

        void Update() {
            if (debugMousePosition) {
                var worldPos = MouseWorld.GetPosition();
                //Debug.Log($"pos:{worldPos} | Grid:{GetGridPosition(worldPos)} | center:{_gridSystemSquare.GetWorldPosition(_gridSystemSquare.GetGridPosition(worldPos))}");
            }

            if (debugGrid) { }
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
            var gridObject = _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition from, GridPosition to) {
            RemoveUnitAtGridPosition(from, unit);
            AddUnitAtGridPosition(to, unit);
            OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) {
            var gridObject = _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition);
            gridObject.Interactable = interactable;
        }

        public IInteractable GetDoorAtGridPosition(GridPosition gridPosition) {
            var gridObject = _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition);
            return gridObject.Interactable;
        }

        private GridSystemSquare<GridObject> GetGridSystem(int floor) {
            return _gridSystemSquare[floor];
        }

        public int GetFloor(Vector3 worldPosition) {
            //Debug.Log($"worldPosition: {worldPosition} - Floor: {Mathf.FloorToInt(worldPosition.y / FloorHeight)}");
            return worldPosition.y <= 0 ? 0 : Mathf.FloorToInt(worldPosition.y / FloorHeight);
        }
        
        //PASS THROUGH TO GRID SYSTEM
        public GridDimension GetGridDimension(int floorNumber) => _gridSystemSquare[floorNumber].GetGridDimension();

        public int GetNumberOfFloors() => numFloors;
        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystemSquare[GetFloor(worldPos)].GetGridPosition(worldPos);
        public Vector3 GetGridWorldPosition(GridPosition gridPosition) => _gridSystemSquare[gridPosition.FloorNumber].GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) {
            if (gridPosition.FloorNumber < 0 || gridPosition.FloorNumber >= numFloors) return false;
            return _gridSystemSquare[gridPosition.FloorNumber].IsValidGridPosition(gridPosition);
        }

        public bool IsGridPositionFree(GridPosition gridPosition) => !_gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition).HasAnyUnit();
        public Unit GetUnitAtGridPosition(GridPosition gridPosition) => _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition).GetUnit();
        public bool IsEnemyAtGridPosition(GridPosition gridPosition, bool isEnemy) => _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition).ContainsEnemy(isEnemy);
        public bool IsInteractableAtGridPosition(GridPosition gridPosition) => _gridSystemSquare[gridPosition.FloorNumber].GetGridObject(gridPosition).Interactable != null;
    }
}