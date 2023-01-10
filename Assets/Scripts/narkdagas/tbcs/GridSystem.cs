using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace narkdagas.tbcs {
    public struct GridPosition : IEquatable<GridPosition> {

        public int X;
        public int Z;

        public GridPosition(int x, int z) {
            this.X = x;
            this.Z = z;
        }

        public override string ToString() {
            return $"[{X},{Z}]";
        }

        public static bool operator == (GridPosition a, GridPosition b) {
            return a.X == b.X && a.Z == b.Z;
        }

        public static bool operator !=(GridPosition a, GridPosition b) {
            return !(a==b);
        }
        
        public bool Equals(GridPosition other) {
            return X == other.X && Z == other.Z;
        }

        public override bool Equals(object obj) {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Z);
        }

    }

    public class GridSystem {
        private int _width;
        private int _length;
        private float _cellSize;
        private GridObject[,] _gridObjects;
        private Transform _debugParent;

        public GridSystem(int width, int length, float cellSize, bool drawGrid = false, Transform debugPrefab = null) {
            _width = width;
            _length = length;
            _cellSize = cellSize;
            _gridObjects = new GridObject[width, length];

            if (debugPrefab) {
                _debugParent = new GameObject("GridDebugObjects").transform;
            }
            
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjects[x, z] = new GridObject(this, gridPosition);

                    if (drawGrid) DebugPaintGridPosition(gridPosition);
                    if (debugPrefab) DebugCreateDebugGridObject(gridPosition, debugPrefab, _debugParent);
                }
            }
        }

        //This returns the center a Vector3 center on the Grid
        private Vector3 GetWorldPosition(int x, int z) {
            return new Vector3(x, .2f, z) * _cellSize;
        }
        
        //This returns the center a Vector3 center on the Grid
        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return GetWorldPosition(gridPosition.X, gridPosition.Z);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            //the origin of the cell is at the center
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / _cellSize), Mathf.RoundToInt(worldPosition.z / _cellSize));
            //the origin of the cell is at the bottom-left corner (watch the borders might seems "bigger"
            //return new GridPosition((int)(worldPosition.x / _cellSize), (int)(worldPosition.z / _cellSize));
        }
        
        public GridObject GetGridObject(GridPosition gridPosition) {
            return IsValidGridPosition(gridPosition) ? _gridObjects[gridPosition.X, gridPosition.Z] : null;
        }

        private bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.X >= 0 &&
                   gridPosition.X < _width &&
                   gridPosition.Z >= 0 &&
                   gridPosition.Z < _length;
        }

        private void DebugPaintGridPosition(GridPosition gridPosition) {
            //Assuming the cell origin is at the center
            var cellCenter = GetWorldPosition(gridPosition);
            var topRightCorner = cellCenter + (new Vector3(.5f, 0, .5f) * _cellSize);
            var bottomLeftCorner = cellCenter + (new Vector3(-.5f, 0, -.5f) * _cellSize);
            //Top
            Debug.DrawLine(topRightCorner, topRightCorner + (Vector3.left * _cellSize), Color.white, 1000);
            //Left
            Debug.DrawLine(bottomLeftCorner, bottomLeftCorner + (Vector3.forward * _cellSize), Color.white, 1000);
            //Right
            Debug.DrawLine(topRightCorner, topRightCorner + (Vector3.back * _cellSize), Color.white, 1000);
            //Bottom
            Debug.DrawLine(bottomLeftCorner, bottomLeftCorner + (Vector3.right * _cellSize), Color.white, 1000);
        }
        
        private void DebugCreateDebugGridObject(GridPosition gridPosition, Transform debugPrefab, Transform parent) {
            var instance = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            instance.SetParent(parent);
            if (instance.TryGetComponent<GridDebugObject>(out var gdo)) {
                gdo.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
}