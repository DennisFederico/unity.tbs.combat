using UnityEngine;

namespace narkdagas.tbcs {
    public struct GridPosition {
        public int x;
        public int z;

        public GridPosition(int x, int z) {
            this.x = x;
            this.z = z;
        }

        public override string ToString() {
            return $"[{x},{z}]";
        }
    }

    public class GridSystem {
        private int _width;
        private int _length;
        private float _cellSize;
        private GridObject[,] _gridObjects;

        public GridSystem(int width, int length, float cellSize, bool debug = false, Transform debugPrefab = null) {
            _width = width;
            _length = length;
            _cellSize = cellSize;

            _gridObjects = new GridObject[width, length];

            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    var gridPosition = new GridPosition(x, z);
                    _gridObjects[x, z] = new GridObject(this, gridPosition);
                    
                    if (debug) {
                        DebugGridPosition(gridPosition, debugPrefab);
                    }
                }
            }
        }

        private Vector3 GetWorldPosition(int x, int z) {
            return new Vector3(x, .2f, z) * _cellSize;
        }
        
        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return GetWorldPosition(gridPosition.x, gridPosition.z);
        }


        public GridPosition GetGridPosition(Vector3 worldPosition) {
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / _cellSize), Mathf.RoundToInt(worldPosition.z / _cellSize));
        }
        
        public GridObject GetGridObject(GridPosition gridPosition) {
            return _gridObjects[gridPosition.x, gridPosition.z];
        }

        private void DebugGridPosition(GridPosition gridPosition, Transform debugPrefab = null) {
            Debug.DrawLine(GetWorldPosition(gridPosition), GetWorldPosition(gridPosition) + Vector3.right * .2f, Color.white, 1000);
                if (debugPrefab != null) InstantiateDebugObject(gridPosition, debugPrefab);
        }
        
        private void InstantiateDebugObject(GridPosition gridPosition, Transform debugPrefab) {
            var instance = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
            if (instance.TryGetComponent<GridDebugObject>(out var gdo)) {
                gdo.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
}