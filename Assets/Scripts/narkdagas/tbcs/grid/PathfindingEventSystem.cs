using System;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class PathfindingEventSystem : MonoBehaviour {
        private void Start() {
            DestructibleCrate.onAnyDestroyed += (sender, _) => {
                var destructible = sender as DestructibleCrate;
                var gridPosition = LevelGrid.Instance.GetGridPosition(destructible.gameObject.transform.position);
                Pathfinding.Instance.SetIsPositionWalkable(gridPosition, true);
            };
        }
    }
}