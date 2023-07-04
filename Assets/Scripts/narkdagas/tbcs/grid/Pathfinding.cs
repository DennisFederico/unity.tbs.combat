using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [SerializeField] private LayerMask walkableLayerMask;
        [SerializeField] private bool debugGrid;
        [SerializeField] private Transform debugPrefab;
        [SerializeField] private Transform pathfindingLinksContainer;
        public const int PathCostMultiplier = 10;
        private GridDimension _gridDimension;
        private readonly List<GridSystemSquare<PathNode>> _gridSystem = new();
        private readonly List<PathfindingLink> _pathfindingLinks = new();
        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one Pathfinding in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            _gridDimension = LevelGrid.Instance.GetGridDimension(0);
            var numFloors = LevelGrid.Instance.GetNumberOfFloors();
            foreach (var floor in Enumerable.Range(0, numFloors)) {
                var gridSystemSquare = new GridSystemSquare<PathNode>(floor,
                    _gridDimension.Width,
                    _gridDimension.Length,
                    _gridDimension.CellSize,
                    LevelGrid.FloorHeight,
                    (_, position) => new PathNode(position),
                    debugGrid,
                    debugPrefab);
                _gridSystem.Add(gridSystemSquare);
            }
            MarkObstacles();
            CreatePathfindingLinks();
        }

        private void MarkObstacles() {
            var numFloors = LevelGrid.Instance.GetNumberOfFloors();
            foreach (var floor in Enumerable.Range(0, numFloors)) {
                for (int x = 0; x < _gridDimension.Width; x++) {
                    for (int z = 0; z < _gridDimension.Length; z++) {

                        var gridPosition = new GridPosition(x, z, floor);

                        //Not walkable by default
                        _gridSystem[floor].GetGridObject(gridPosition).IsWalkable = false;

                        var worldPosition = LevelGrid.Instance.GetGridWorldPosition(gridPosition);
                        float raycastOffset = .3f;

                        //RayCast down to see if there's a walkable layer below
                        if (Physics.Raycast(worldPosition + (Vector3.up * raycastOffset), Vector3.down, raycastOffset * 2, walkableLayerMask)) {
                            _gridSystem[floor].GetGridObject(gridPosition).IsWalkable = true;
                        }

                        //RayCast up to see if there's an obstacle above
                        if (Physics.Raycast(worldPosition + (Vector3.down * raycastOffset), Vector3.up, raycastOffset * 2, obstaclesLayerMask)) {
                            _gridSystem[floor].GetGridObject(gridPosition).IsWalkable = false;
                        }
                    }
                }
            }
        }
        
        private void CreatePathfindingLinks() {
            foreach (var pathfindingLinkMono in pathfindingLinksContainer.GetComponentsInChildren<PathfindingLinkMono>()) {
                _pathfindingLinks.Add(pathfindingLinkMono.GetPathfindingLink());
            }
        }

        public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathCost) {
            ResetGrid();

            List<PathNode> openList = new();
            List<PathNode> closedList = new();

            PathNode startNode = _gridSystem[startGridPosition.FloorNumber].GetGridObject(startGridPosition);
            PathNode endNode = _gridSystem[endGridPosition.FloorNumber].GetGridObject(endGridPosition);

            openList.Add(startNode);

            startNode.GCost = 0;
            startNode.HCost = CalculateDistanceCost(startGridPosition, endGridPosition);
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

                var neighbours = GetNeighbourList(currentNode);
                neighbours.AddRange(GetLinkedGridPositions(currentNode.GridPosition)
                    .Select(gridPosition => _gridSystem[gridPosition.FloorNumber].GetGridObject(gridPosition)));
                
                //Get neighbours
                foreach (PathNode neighbourNode in neighbours) {
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.IsWalkable) continue;
                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode.GridPosition, neighbourNode.GridPosition);
                    if (tentativeGCost < neighbourNode.GCost) {
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode.GridPosition, endNode.GridPosition);
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
        
        private List<GridPosition> GetLinkedGridPositions(GridPosition gridPosition) {
            List<GridPosition> gridPositions = new();
            //This could be faster if we used a dictionary with a list of the links by current floor or even the actual grid position as the key
            foreach (var link in _pathfindingLinks) {
                if (link.GridPositionA == gridPosition) {
                    gridPositions.Add(link.GridPositionB);
                } else if (link.GridPositionB == gridPosition) {
                    gridPositions.Add(link.GridPositionA);
                }
            }

            return gridPositions;
        }

        private int CalculateDistanceCost(GridPosition a, GridPosition b) {
            GridPosition distance = a - b;
            int h = (int)(Mathf.Sqrt(distance.X * distance.X + distance.Z * distance.Z) * PathCostMultiplier);
            return h;
        }

        private List<PathNode> GetNeighbourList(PathNode pathNode) {
            List<PathNode> neighbours = new();
            var currentGridPosition = pathNode.GridPosition;

            var neighboursGridPositions = GridPosition.GetSquareGridPositionsNeighbours(0);
            // neighboursGridPositions.AddRange(GridPosition.GetSquareGridPositionsNeighbours(1));
            // neighboursGridPositions.AddRange(GridPosition.GetSquareGridPositionsNeighbours(-1));
            
            foreach (GridPosition neighbour in neighboursGridPositions) {
                var nextGridPosition = currentGridPosition + neighbour;
                if (nextGridPosition.FloorNumber < 0 || nextGridPosition.FloorNumber >= _gridSystem.Count) continue;
                if (_gridSystem[nextGridPosition.FloorNumber].IsValidGridPosition(nextGridPosition)) {
                    neighbours.Add(_gridSystem[nextGridPosition.FloorNumber].GetGridObject(nextGridPosition));
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
            return _gridSystem[gridPosition.FloorNumber].GetGridObject(gridPosition).IsWalkable;
        }

        public void SetIsPositionWalkable(GridPosition gridPosition, bool isWalkable) {
            _gridSystem[gridPosition.FloorNumber].GetGridObject(gridPosition).IsWalkable = isWalkable;
        }

        public bool HasPath(GridPosition startPosition, GridPosition targetPosition) {
            return FindPath(startPosition, targetPosition, out _) != null;
        }

        private void ResetGrid() {
            // Reset Initialize the list
            var numFloors = LevelGrid.Instance.GetNumberOfFloors();
            foreach (var floor in Enumerable.Range(0, numFloors)) {
                for (int x = 0; x < _gridDimension.Width; x++) {
                    for (int z = 0; z < _gridDimension.Length; z++) {
                        GridPosition position = new GridPosition(x, z, floor);
                        PathNode pathNode = _gridSystem[position.FloorNumber].GetGridObject(position);
                        pathNode.GCost = int.MaxValue;
                        pathNode.HCost = 0;
                        pathNode.UpdateFCost();
                        pathNode.PrevNodeInPath = null;
                    }
                }
            }
        }
    }
}