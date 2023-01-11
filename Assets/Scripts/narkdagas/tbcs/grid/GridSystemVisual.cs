using System;
using System.Collections.Generic;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridSystemVisual : MonoBehaviour {
        
        public static GridSystemVisual Instance { get; private set; }
        
        [SerializeField] private Transform gridVisualSingle;
        private GridSystemVisualSingle[,] _gridVisualsArray;
        private Transform _visualsParent;

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
        }

        private void Update() {
            UpdateGridVisual();
        }

        public void HideAllGridVisuals() {
            foreach (var visual in _gridVisualsArray) {
                visual.Hide();
            }
        }

        public void ShowGridPositionsVisuals(List<GridPosition> gridPositionList) {
            foreach (var position in gridPositionList) {
                _gridVisualsArray[position.X, position.Z].Show();
            }            
        }

        private void UpdateGridVisual() {
            var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit) {
                HideAllGridVisuals();
                var gridList = UnitActionSystem.Instance.GetSelectedAction().GetValidActionGridPositionList();
                ShowGridPositionsVisuals(gridList);                    
            }
        }
    }
}