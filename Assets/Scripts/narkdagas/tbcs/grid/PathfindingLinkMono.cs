using UnityEngine;

namespace narkdagas.tbcs.grid {
    public struct PathfindingLink {
        public GridPosition GridPositionA;
        public GridPosition GridPositionB;
        public int Cost;
    }

    public class PathfindingLinkMono : MonoBehaviour {
        public Vector3 positionA;
        public Vector3 positionB;

        public PathfindingLink GetPathfindingLink() {
            return new PathfindingLink() {
                GridPositionA = LevelGrid.Instance.GetGridPosition(positionA),
                GridPositionB = LevelGrid.Instance.GetGridPosition(positionB),
                Cost = 10
            };
        }
    }
}