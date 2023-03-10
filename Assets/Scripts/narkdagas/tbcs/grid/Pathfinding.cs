using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace narkdagas.tbcs.grid {

    public class PathNode {
        public readonly GridPosition GridPosition;

        public PathNode(GridPosition gridPosition) {
            GridPosition = gridPosition;
        }

        public int GCost { get; set; } = int.MaxValue;

        public int HCost { get; set; }

        public int FCost { get; private set; } = int.MaxValue;

        public void UpdateFCost() {
            FCost = GCost + HCost;
        }

        public bool IsWalkable { get; set; } = true;

        public PathNode PrevNodeInPath { get; set; }

        public override string ToString() {
            return GridPosition.ToString();
        }
    }
    
    public class Pathfinding : MonoBehaviour {
        public static Pathfinding Instance { get; private set; }
        [SerializeField] private LayerMask obstaclesLayerMask;
        [SerializeField] private bool debugGrid;
        [SerializeField] private Transform debugPrefab;
        [SerializeField] private bool isHexGrid;
        public const int PathCostMultiplier = 10;
        private GridDimension _gridDimension;
        //TODO TYPE OF GRID SQUARE vs. HEX
        private GridSystemHex<PathNode> _gridSystemHex;
        

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one Pathfinding in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start() {
            _gridDimension = LevelGrid.Instance.GetGridDimension();
            _gridSystemHex = new GridSystemHex<PathNode>(_gridDimension.Width, 
                _gridDimension.Length, 
                _gridDimension.CellSize,
                ((_, position) => new PathNode(position)),
                debugGrid,
                debugPrefab);
            
            MarkObstacles();
        }

        private void MarkObstacles() {
            for (int x = 0; x < _gridDimension.Width; x++) {
                for (int z = 0; z < _gridDimension.Length; z++) {
                    var gridPosition = new GridPosition(x, z);
                    var worldPosition = LevelGrid.Instance.GetGridWorldPosition(gridPosition);
                    float raycastOffset = .2f;
                    if (Physics.Raycast(worldPosition + (Vector3.down * raycastOffset), Vector3.up, 1f, obstaclesLayerMask)) {
                        _gridSystemHex.GetGridObject(gridPosition).IsWalkable = false;
                    }
                }
            }
        }

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathCost) {
            ResetGrid();
            
            List<PathNode> openList = new();
            List<PathNode> closedList = new();

            PathNode startNode = _gridSystemHex.GetGridObject(startGridPosition);
            PathNode endNode = _gridSystemHex.GetGridObject(endGridPosition);
            
            openList.Add(startNode);

            startNode.GCost = 0;
            startNode.HCost = CalculateHeuristicDistanceCost(startGridPosition, endGridPosition);
            startNode.UpdateFCost();

            while (openList.Count > 0) {
                openList.Sort((a, b) => a.FCost - b.FCost);
                PathNode currentNode = openList[0];
                
                if (currentNode == endNode) {
                    //Reached the end found path
                    pathCost = endNode.FCost;
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);
                
                //Get neighbours
                List<PathNode> neighbours = isHexGrid ? GetHexGridNeighbourList(currentNode) : GetSquareGridNeighbourList(currentNode); 
                foreach (PathNode neighbourNode in neighbours) {
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.IsWalkable) continue;
                    int tentativeGCost = currentNode.GCost + (isHexGrid ? PathCostMultiplier : CalculateHeuristicDistanceCost(currentNode.GridPosition, neighbourNode.GridPosition));
                    if (tentativeGCost < neighbourNode.GCost) {
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateHeuristicDistanceCost(neighbourNode.GridPosition, endNode.GridPosition);
                        neighbourNode.UpdateFCost();
                        neighbourNode.PrevNodeInPath = currentNode;
                        if (!openList.Contains(neighbourNode)) openList.Add(neighbourNode);
                    }
                }
            }
            //No path found
            pathCost = 0;
            return null;
        }

        private int CalculateHeuristicDistanceCost(GridPosition a, GridPosition b) {
            if (isHexGrid) {
                return (int) (Vector3.Distance(_gridSystemHex.GetWorldPosition(a), _gridSystemHex.GetWorldPosition(b)) * PathCostMultiplier);
            }
            GridPosition distance = a - b;
            int h = (int) (Mathf.Sqrt(distance.X * distance.X + distance.Z * distance.Z) * PathCostMultiplier);
            return h;
        }

        private List<PathNode> GetSquareGridNeighbourList(PathNode pathNode) {
            List<PathNode> neighbours = new();
            var currentGridPosition = pathNode.GridPosition;

            foreach (GridPosition neighbour in GridPosition.GetSquareGridPositionsNeighbours()) {
                var nextGridPosition = currentGridPosition + neighbour;
                if (_gridSystemHex.IsValidGridPosition(nextGridPosition)) {
                    neighbours.Add(_gridSystemHex.GetGridObject(nextGridPosition));
                }
            }

            return neighbours;
        }
        
        private List<PathNode> GetHexGridNeighbourList(PathNode pathNode) {
            List<PathNode> neighbours = new();
            var currentGridPosition = pathNode.GridPosition;

            foreach (GridPosition neighbour in GridPosition.GetHexGridPositionsNeighbours(currentGridPosition.Z % 2 == 1)) {
                var nextGridPosition = currentGridPosition + neighbour;
                if (_gridSystemHex.IsValidGridPosition(nextGridPosition)) {
                    neighbours.Add(_gridSystemHex.GetGridObject(nextGridPosition));
                }
            }

            return neighbours;
        }
        
        private List<GridPosition> CalculatePath(PathNode endNode) {
            List<GridPosition> gridPositions = new();
            var currentNode = endNode;
            
            gridPositions.Add(currentNode.GridPosition);
            while (currentNode.PrevNodeInPath != null) {
                currentNode = currentNode.PrevNodeInPath;
                gridPositions.Add(currentNode.GridPosition);
            }
            gridPositions.Reverse();
            return gridPositions;
        }

        public bool IsPositionWalkable(GridPosition gridPosition) {
            return _gridSystemHex.GetGridObject(gridPosition).IsWalkable;
        }
        
        public void SetIsPositionWalkable(GridPosition gridPosition, bool isWalkable) {
            _gridSystemHex.GetGridObject(gridPosition).IsWalkable = isWalkable;
        }

        public bool HasPath(GridPosition startPosition, GridPosition targetPosition) {
            return FindPath(startPosition, targetPosition, out _) != null;
        }

        private void ResetGrid() {
            // Reset Initialize the list
            for (int x = 0; x < _gridDimension.Width; x++) {
                for (int z = 0; z < _gridDimension.Length; z++) {
                    GridPosition position = new GridPosition(x, z);
                    PathNode pathNode = _gridSystemHex.GetGridObject(position);
                    pathNode.GCost = int.MaxValue;
                    pathNode.HCost = 0;
                    pathNode.UpdateFCost();
                    pathNode.PrevNodeInPath = null;
                }
            }    
        }
    }
}