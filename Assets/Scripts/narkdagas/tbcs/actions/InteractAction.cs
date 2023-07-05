using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;

namespace narkdagas.tbcs.actions {
    public class InteractAction : BaseAction {

        private readonly int _maxInteractDistance = 1;
        
        public override string GetActionNameLabel() {
            return "Interact";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            var door = LevelGrid.Instance.GetDoorAtGridPosition(gridPosition);
            ActionStart(onActionComplete, EventArgs.Empty);
            door.Interact(ActionComplete);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            
            for (int x = -_maxInteractDistance; x <= _maxInteractDistance; x++) {
                for (int z = -_maxInteractDistance; z <= _maxInteractDistance; z++) {
                    GridPosition gridPositionCandidate = Unit.GetGridPosition() + new GridPosition(x, z, 0);
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        LevelGrid.Instance.IsInteractableAtGridPosition(gridPositionCandidate) &&
                        LevelGrid.Instance.IsGridPositionFree(gridPositionCandidate)) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            return validGridPositionList;
        }

        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            return new EnemyAIActionData() {
                GridPosition = gridPosition,
                ActionValue = 0
            };
        }
    }
}