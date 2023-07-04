using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class MoveAction : BaseAction {
        public event EventHandler MoveActionStarted;
        public event EventHandler MoveActionCompleted;
        [SerializeField] private int maxMoveGridDistance = 4;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float stoppingAngle = .1f;

        private List<Vector3> _targetPositionsList;
        private int _currentPositionIndex;

        public override string GetActionNameLabel() {
            return "Move";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            MoveActionStarted?.Invoke(this, EventArgs.Empty);
            Move(gridPosition);
            ActionStart(onActionComplete, EventArgs.Empty);
        }

        void Update() {
            if (!IsActive) return;
            
            Vector3 targetPosition = _targetPositionsList[_currentPositionIndex];
            Vector3 targetDirection = (_targetPositionsList[_currentPositionIndex] - transform.position).normalized;
            //First Rotate
            var angle = Vector3.Angle(transform.forward, targetDirection);
            if (angle > stoppingAngle) {
                transform.forward = Vector3.Lerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
            }

            //Only start to move if the facing direction is < 90º
            
            if (angle < 90) {
                if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {
                    transform.position += targetDirection * (moveSpeed * Time.deltaTime);
                } else {
                    _currentPositionIndex++;
                    if (_currentPositionIndex >= _targetPositionsList.Count) {
                        ActionComplete();
                        MoveActionCompleted?.Invoke(this, EventArgs.Empty);    
                    }
                }
            }
        }

        private void Move(GridPosition gridPosition) {
            var pathGridPositions = Pathfinding.Instance.FindPath(Unit.GetGridPosition(), gridPosition, out int _);
            _currentPositionIndex =  0;
            _targetPositionsList = new List<Vector3>();
            foreach (var pathGridPosition in pathGridPositions) {
                _targetPositionsList.Add(LevelGrid.Instance.GetGridWorldPosition(pathGridPosition));
            }
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            var unitGridPosition = Unit.GetGridPosition();

            for (int floor = -maxMoveGridDistance; floor <= maxMoveGridDistance; floor++) {
                for (int x = -maxMoveGridDistance; x <= maxMoveGridDistance; x++) {
                    for (int z = -maxMoveGridDistance; z <= maxMoveGridDistance; z++) {
                        GridPosition gridPositionCandidate = unitGridPosition + new GridPosition(x, z, floor);
                        if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                            LevelGrid.Instance.IsGridPositionFree(gridPositionCandidate) &&
                            Pathfinding.Instance.IsPositionWalkable(gridPositionCandidate) &&
                            Pathfinding.Instance.HasPath(unitGridPosition, gridPositionCandidate) &&
                            GetPathCost(unitGridPosition, gridPositionCandidate) <= maxMoveGridDistance * Pathfinding.PathCostMultiplier) {
                            validGridPositionList.Add(gridPositionCandidate);
                        }
                    }
                }
            }

            return validGridPositionList;
        }

        private int GetPathCost(GridPosition startGridPosition, GridPosition endGridPosition) {
            Pathfinding.Instance.FindPath(startGridPosition, endGridPosition, out int pathCost);
            return pathCost;
        }
        
        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            var targetCount = Unit.GetAction<ShootAction>().GetTargetCountAtGridPosition(gridPosition);
            
            return new EnemyAIActionData {
                GridPosition = gridPosition,
                ActionValue = targetCount * 10
            };
        }
    }
}