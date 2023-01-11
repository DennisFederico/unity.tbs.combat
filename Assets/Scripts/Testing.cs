using narkdagas.tbcs;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace DefaultNamespace {
    public class Testing : MonoBehaviour {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.T)) {
                var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
                if (selectedUnit != null) {
                    var list = selectedUnit.GetMoveAction().GetValidActionGridPositionList();
                    GridSystemVisual.Instance.HideAllGridVisuals();
                    GridSystemVisual.Instance.ShowGridPositionsVisuals(list);                    
                }
            }
        }
    }
}