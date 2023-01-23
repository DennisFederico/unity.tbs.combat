using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace narkdagas.tbcs.grid {
    public class GridSystemHex<TGridObject> where TGridObject : class {
        private readonly GridDimension _gridDimension;

        //private float _cellSize;
        private readonly TGridObject[,] _gridObjects;
        private readonly Transform _debugParent;
        private const float ZOffset = 0.75f;
        private const float XOffset = 0.5f;

        public GridSystemHex(int width, int length, float cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObjectFunction, bool drawGrid = false,
            Transform debugPrefab = null) {
            _gridDimension = new GridDimension(width, length, cellSize);
            _gridObjects = new TGridObject[width, length];

            if (debugPrefab) {
                _debugParent = new GameObject("GridDebugObjects").transform;
            }

            for (int x = 0; x < width; x++) {
                for (int z = 0; z < length; z++) {
                    var gridPosition = new GridPosition(x, z);
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
        public Vector3 GetWorldPosition(GridPosition gridPosition) {
            return GetWorldPosition(gridPosition.X, gridPosition.Z);
        }

        private Vector3 GetWorldPosition(int x, int z) {
            float xOffset = (z % 2) * XOffset;
            return new Vector3(x + xOffset, .0001f, z * ZOffset) * _gridDimension.CellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) {
            var aproxGridPosition = new GridPosition(
                Mathf.RoundToInt(worldPosition.x / _gridDimension.CellSize),
                Mathf.RoundToInt(worldPosition.z / _gridDimension.CellSize / ZOffset)
                );
            
            var hexGridPositionsNeighbours = GridPosition.GetHexGridPositionsNeighbours(aproxGridPosition.Z % 2 == 1);
            
            float distance = Vector3.Distance(worldPosition, GetWorldPosition(aproxGridPosition));
            GridPosition closest = aproxGridPosition;
            foreach (GridPosition neighbour in hexGridPositionsNeighbours) {
                GridPosition gridCandidate = aproxGridPosition + neighbour;
                var newDistance = Vector3.Distance(worldPosition, GetWorldPosition(gridCandidate));
                if (newDistance < distance) {
                    distance = newDistance;
                    closest = gridCandidate;
                }
            }

            return closest;
        }

        public bool IsValidGridPosition(GridPosition gridPosition) {
            return gridPosition.X >= 0 &&
                   gridPosition.X < _gridDimension.Width &&
                   gridPosition.Z >= 0 &&
                   gridPosition.Z < _gridDimension.Length;
        }

        public TGridObject GetGridObject(GridPosition gridPosition) {
            return IsValidGridPosition(gridPosition) ? _gridObjects[gridPosition.X, gridPosition.Z] : null;
        }

        //TODO MOVE DEBUG STUFF TO A "PARTIAL" FILE?
        private void DebugPaintGridPosition(GridPosition gridPosition) {
            //Assuming the cell origin is at the center
            var gridCenterPosition = GetWorldPosition(gridPosition);
            Vector3 cellCenter = new Vector3(gridCenterPosition.x, 0, gridCenterPosition.z);
            float halfCellSize = _gridDimension.CellSize / 2f;
            //Each side is the base of a triangle with opposed vertex at the center
            //Hexagon inner angles are 120 and sum 720... inner angles of a triangle sum 180, thus 60 from the opposed vertex
            // c^2 = a^2 + b^2 --> c^2 - b^2 = a^2 => height of the triangle
            //Start for center-top minus half size
            var height = Mathf.Sqrt(_gridDimension.CellSize * _gridDimension.CellSize - halfCellSize * halfCellSize);
            Vector3 start = (cellCenter + (Vector3.forward * height)) + Vector3.left * halfCellSize;
            Vector3 end = start + Vector3.right * _gridDimension.CellSize;
            //TOP
            Debug.DrawLine(start, end, Color.white, 1000);
            //ROTATE 60 degrees over center?
        }

        private void DebugCreateDebugGridObject(GridPosition gridPosition, Transform debugPrefab, Transform parent) {
            var instance = Object.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, parent);
            if (instance.TryGetComponent<GridDebugObject>(out var gdo)) {
                gdo.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
}