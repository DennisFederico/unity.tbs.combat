using System;
using System.Collections.Generic;
using UnityEngine;

namespace narkdagas.tbcs {
    public class LevelGrid : MonoBehaviour {

        public static LevelGrid Instance { get; private set; }
        
        [SerializeField] private bool debugGrid;
        [SerializeField] private bool debugMousePosition;
        [SerializeField] private Transform debugPrefab;
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridLenght;
        [SerializeField] private int gridCellSize;
        private GridSystem _gridSystem;
        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one LevelGrid in the scene! {transform} -{Instance}");
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

            if (debugGrid) {
                
            }
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
        }

        public GridPosition GetGridPosition(Vector3 worldPos) => _gridSystem.GetGridPosition(worldPos);
        
        
        // private void DebugGridObjectInstance(GridPosition gridPosition, Transform debugPrefab) {
        //     var instance = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
        //     if (instance.TryGetComponent<GridDebugObject>(out var gdo)) {
        //         gdo.SetGridObject(GetGridObject(gridPosition));
        //     }
        // }
    }
}