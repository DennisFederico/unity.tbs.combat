using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace narkdagas.tbcs.grid {
    

    public class GridSystemSquare<TGridObject> where TGridObject : class {
        private readonly int _floorNumber;
        private readonly float _floorHeight;
        private readonly GridDimension _gridDimension;
        private readonly TGridObject[,] _gridObjects;
        private readonly Transform _debugParent;

        public GridSystemSquare(int floorNumber, int width, int length, float cellSize, float floorHeight, Func<GridSystemSquare<TGridObject>, GridPosition, TGridObject> createGridObjectFunction, bool drawGrid = false, Transform debugPrefab = null) {
            _floorNumber = floorNumber;
            _floorHeight = floorHeight;
            _gridDimension = new GridDimension(width, length, cellSize);
            _gridObjects = new TGridObject[width, length];

            if (debugPrefab) {
                _debugParent = new GameObject("GridDebugObjects").transform;
            }

            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    var gridPosition = new GridPosition(x, z, _floorNumber);
                    _gridObjects[x, z] = createGridObjectFunction(this, gridPosition);

                    if (drawGrid) DebugPaintGridPosition(gridPosition);
                    if (debugPrefab) DebugCreateDebugGridObject(gridPosition, debugPrefab, _debugParent);
                }
            }
        }

        public GridDimension GetGridDimension() {
            return _gridDimension;
        }

        //This returns the center a Vector3 center on the Grid
        private Vector3 GetWorldPosition(int x, int z, int floorNumber) {
            //return new Vector3(x, .0001f, z) * _gridDimension.CellSize;
            return new Vector3(x, .0001f, z) * _gridDimension.CellSize +
                   new Vector3(0, floorNumber * _floorHeight, 0);
        }

        //This returns the center a Vector3 center on the Grid
        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return GetWorldPosition(gridPosition.X, gridPosition.Z, gridPosition.FloorNumber);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            //the origin of the cell is at the center
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x / _gridDimension.CellSize), 
                Mathf.RoundToInt(worldPosition.z / _gridDimension.CellSize),
                _floorNumber
                );
            //the origin of the cell is at the bottom-left corner (watch the borders might seems "bigger"
            //return new GridPosition((int)(worldPosition.x / _cellSize), (int)(worldPosition.z / _cellSize));
        }

        public bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.X >= 0 &&
                   gridPosition.X < _gridDimension.Width &&
                   gridPosition.Z >= 0 &&
                   gridPosition.Z < _gridDimension.Length &&
                   gridPosition.FloorNumber == _floorNumber;
        }

        public TGridObject GetGridObject(GridPosition gridPosition) {
            return IsValidGridPosition(gridPosition) ? _gridObjects[gridPosition.X, gridPosition.Z] : null;
        }

        //TODO MOVE DEBUG STUFF TO A "PARTIAL" FILE?
        private void DebugPaintGridPosition(GridPosition gridPosition) {
            //Assuming the cell origin is at the center
            var cellCenter = GetWorldPosition(gridPosition);
            var topRightCorner = cellCenter + (new Vector3(.5f, 0, .5f) * _gridDimension.CellSize);
            var bottomLeftCorner = cellCenter + (new Vector3(-.5f, 0, -.5f) * _gridDimension.CellSize);
            //Top
            Debug.DrawLine(topRightCorner, topRightCorner + (Vector3.left * _gridDimension.CellSize), Color.white, 1000);
            //Left
            Debug.DrawLine(bottomLeftCorner, bottomLeftCorner + (Vector3.forward * _gridDimension.CellSize), Color.white, 1000);
            //Right
            Debug.DrawLine(topRightCorner, topRightCorner + (Vector3.back * _gridDimension.CellSize), Color.white, 1000);
            //Bottom
            Debug.DrawLine(bottomLeftCorner, bottomLeftCorner + (Vector3.right * _gridDimension.CellSize), Color.white, 1000);
        }

        private void DebugCreateDebugGridObject(GridPosition gridPosition, Transform debugPrefab, Transform parent) {
            var instance = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parent);
            if (instance.TryGetComponent<GridDebugObject>(out var gdo)) {
                gdo.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
}