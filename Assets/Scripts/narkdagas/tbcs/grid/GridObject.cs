using System;
using System.Collections.Generic;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.unit;

namespace narkdagas.tbcs.grid {
    
    public class GridObject {

        public static Func<GridSystemSquare<GridObject>, GridPosition, GridObject> CtorFunction = (system, position) => new GridObject(system, position);

        //TODO these should be set by the Grid System when adding the GridObject
        private GridSystemSquare<GridObject> gridSystemSquare;

        private GridPosition _gridPosition;

        //TODO This should be added to specializations/extensions to the GridObject
        private readonly List<Unit> _unitList;
        //TODO Use Interactable instead of door
        public IInteractable Interactable { get; set; }

        public GridObject(GridSystemSquare<GridObject> gridSystemSquare, GridPosition gridPosition) {
            this.gridSystemSquare = gridSystemSquare;
            _gridPosition = gridPosition;
            _unitList = new();
        }

        public override string ToString() {
            string units = "";
            foreach (var unit in _unitList) {
                units += $"\n{unit}";
            }

            return $"{_gridPosition.ToString()}{units}";
        }

        public void AddUnit(Unit unit) {
            _unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit) {
            _unitList.Remove(unit);
        }

        public List<Unit> GetUnitList() {
            return _unitList;
        }

        //Given the current game design, there could only be a single unit per grid position
        //at any given time when an action is taken
        public Unit GetUnit() {
            return HasAnyUnit() ? _unitList[0] : null;
        }

        public bool ContainsEnemy(bool isPlayer) {
            return HasAnyUnit() && _unitList[0].IsEnemyUnit() != isPlayer;
        }

        public bool HasAnyUnit() {
            return _unitList.Count != 0;
        }
    }
}