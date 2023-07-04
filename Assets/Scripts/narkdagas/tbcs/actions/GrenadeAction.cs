using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class GrenadeAction : BaseAction {
        [SerializeField] private Transform grenadeProjectilePrefab;
        [SerializeField] private int maxThrowDistance = 7;
        [SerializeField] private int blastRadius = 3;
        [SerializeField] private int damage = 30;
        //TODO IMPLEMENT NUMBER OF GRENADES
        
        public override string GetActionNameLabel() {
            return "Grenade";
        }

        private void Update() {
            if (!IsActive) return;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            var grenade = Instantiate(grenadeProjectilePrefab, Unit.GetWorldPosition() + Vector3.up, Quaternion.identity);
            var grenadeProjectile = grenade.GetComponent<GrenadeProjectile>();
            grenadeProjectile.ThrowTo(gridPosition, blastRadius, damage, ActionComplete);
            ActionStart(onActionComplete, EventArgs.Empty);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            return GetValidActionGridPositionList(Unit.GetGridPosition());
        }
        
        public List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition) {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            
            for (int x = -maxThrowDistance; x <= maxThrowDistance; x++) {
                for (int z = -maxThrowDistance; z <= maxThrowDistance; z++) {
                    GridPosition gridPositionCandidate = gridPosition % new GridPosition(x, z, 0);
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        IsInThrowDistance(x, z, maxThrowDistance)) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            return validGridPositionList;
        }
        
        private bool IsInThrowDistance(int x, int z, int shootDistance) {
            return Mathf.Sqrt(x * x + z * z) <= shootDistance;
        }

        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            return new EnemyAIActionData {
                GridPosition = gridPosition,
                //TODO Evaluate by the number of affected units
                ActionValue = 0
            };
        }
    }
}