namespace narkdagas.tbcs {
    
    public class GridObject {
        private GridSystem _gridSystem;
        public GridPosition GridPosition;

        public GridObject(GridSystem gridSystem, GridPosition gridPosition) {
            _gridSystem = gridSystem;
            GridPosition = gridPosition;
        }

        public override string ToString() {
            return GridPosition.ToString();
        }
    }
}