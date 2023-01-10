using System.Collections.Generic;

namespace narkdagas.tbcs {
    
    public class GridObject {
        //TODO these should be set by the Grid System when adding the GridObject
        protected GridSystem GridSystem;
        protected GridPosition GridPosition;
        //TODO This should be added to specializations/extensions to the GridObject
        protected List<Unit> UnitList;
        
        public GridObject(GridSystem gridSystem, GridPosition gridPosition) {
            GridSystem = gridSystem;
            GridPosition = gridPosition;
            UnitList = new();
        }

        public override string ToString() {
            string units = "";
            foreach (var unit in UnitList) {
                units += $"\n{unit}";
            }
            return $"{GridPosition.ToString()}{units}";
        }

        public void AddUnit(Unit unit) {
            UnitList.Add(unit);
        }

        public void RemoveUnit(Unit unit) {
            UnitList.Remove(unit);
        }

        public List<Unit> GetUnitList() {
            return UnitList;
        }
    }
}