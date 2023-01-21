using System;
using System.Collections.Generic;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridSystemVisual : MonoBehaviour {
        public static GridSystemVisual Instance { get; private set; }

        [SerializeField] private Transform gridVisualSingle;
        private GridSystemVisualSingle[,] _gridVisualsArray;
        private Transform _visualsParent;
        [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialsList;
        
        public enum GridVisualType {
            White, Blue, Red, RedSoft, Yellow
        }

        [Serializable]
        public struct GridVisualTypeMaterial {
            public GridVisualType gridVisualType;
            public Material material;
        }
        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one GridSystemVisual in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            var gridDimension = LevelGrid.Instance.GetGridDimension();
            _gridVisualsArray = new GridSystemVisualSingle[gridDimension.Width, gridDimension.Length];
            _visualsParent = new GameObject("GridVisuals").transform;
            for (int x = 0; x < gridDimension.Width; x++) {
                for (int z = 0; z < gridDimension.Length; z++) {
                    var gridVisual = Instantiate(gridVisualSingle, LevelGrid.Instance.GetGridWorldPosition(new GridPosition(x, z)), Quaternion.identity, _visualsParent);
                    _gridVisualsArray[x, z] = gridVisual.GetComponent<GridSystemVisualSingle>();
                }
            }
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
            UpdateGridVisual();
        }

        public void HideAllGridVisuals() {
            foreach (var visual in _gridVisualsArray) {
                visual.Hide();
            }
        }

        public void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType) {
            
            List<GridPosition> gridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++) {
                for (int z = -range; z <= range; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + gridPosition;
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        Mathf.Sqrt(x * x + z * z) <= range ) {
                        gridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            ShowGridPositionsVisuals(gridPositionList, gridVisualType);
        }
        
        public void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType) {
            
            List<GridPosition> gridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++) {
                for (int z = -range; z <= range; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + gridPosition;
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate)) {
                        gridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            ShowGridPositionsVisuals(gridPositionList, gridVisualType);
        }

        public void ShowGridPositionsVisuals(List<GridPosition> gridPositionList, GridVisualType gridVisualType) {
            foreach (var position in gridPositionList) {
                _gridVisualsArray[position.X, position.Z].Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }

        private void UpdateGridVisual() {
            var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (!selectedUnit) return;
            HideAllGridVisuals();
            var selectedAction = UnitActionSystem.Instance.GetSelectedAction();
            if (!selectedAction) return;
            var gridList = UnitActionSystem.Instance.GetSelectedAction().GetValidActionGridPositionList();
            GridVisualType gridVisualType = GridVisualType.White;
            switch (selectedAction) {
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetShootRange(), GridVisualType.RedSoft);
                    break;
                default:
                case MoveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case GrenadeAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case SwordAction swordAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetSwordActionRange(), GridVisualType.RedSoft);
                    break;
                case InteractAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
            }
            ShowGridPositionsVisuals(gridList, gridVisualType);
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs args) {
            UpdateGridVisual();
        }

        private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs args) {
            UpdateGridVisual();
        }

        private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) {
            foreach (var gridVisualTypeMaterial in gridVisualTypeMaterialsList) {
                if (gridVisualTypeMaterial.gridVisualType == gridVisualType) {
                    return gridVisualTypeMaterial.material;
                }
            }
            Debug.LogError($"Couldn't find a GridVisualTypeMaterial for GridVisualType {gridVisualType} ");
            return null;
        }
    }
}