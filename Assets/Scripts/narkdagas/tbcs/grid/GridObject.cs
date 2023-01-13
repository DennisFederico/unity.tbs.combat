using System.Collections.Generic;
using Unity.VisualScripting;
using Unit = narkdagas.tbcs.unit.Unit;

namespace narkdagas.tbcs.grid {
    public class GridObject {
        //TODO these should be set by the Grid System when adding the GridObject
        private GridSystem _gridSystem;

        private GridPosition _gridPosition;

        //TODO This should be added to specializations/extensions to the GridObject
        private readonly List<Unit> _unitList;

        public GridObject(GridSystem gridSystem, GridPosition gridPosition) {
            _gridSystem = gridSystem;
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