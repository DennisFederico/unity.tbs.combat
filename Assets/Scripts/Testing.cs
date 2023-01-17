using narkdagas.tbcs;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace DefaultNamespace {
    public class Testing : MonoBehaviour {
        // private void Update() {
        //     if (Input.GetKeyDown(KeyCode.T)) {
        //         GridPosition startGridPosition = new GridPosition(0, 0);
        //         GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        //
        //         GridPosition? prev = null;
        //         foreach (var gridPosition in Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition, out int pathCost)) {
        //             if (prev == null) {
        //                 prev = gridPosition;
        //                 continue;
        //             }
        //
        //             Debug.DrawLine(
        //                 LevelGrid.Instance.GetGridWorldPosition(prev.Value),
        //                 LevelGrid.Instance.GetGridWorldPosition(gridPosition),
        //                 Color.black,
        //                 10f
        //                 );
        //             prev = gridPosition;
        //         }
        //     }
        // }
    }
}