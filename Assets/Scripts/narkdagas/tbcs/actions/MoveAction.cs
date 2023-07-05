using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class MoveAction : BaseAction {
        public event EventHandler MoveActionStarted;
        public event EventHandler MoveActionCompleted;
        public event EventHandler<JumpActionEventArgs> JumpActionStarted;
        
        public class JumpActionEventArgs : EventArgs {
            public GridPosition CurrentGridPosition;
            public GridPosition TargetGridPosition;
        }
        
        [SerializeField] private int maxMoveGridDistance = 4;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float stoppingAngle = .1f;

        private List<Vector3> _targetPositionsList;
        private int _currentPositionIndex;
        private bool _verticalMovement;
        private float _verticalMovementTimer;
        private float _verticalMovementDuration = .5f;

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

            if (_verticalMovement) {
                //Vertical movement logic
                //Face the target position on the same level
                var targetPositionOnSameLevel = targetPosition;
                targetPositionOnSameLevel.y = transform.position.y;
                Vector3 targetDirection = (targetPositionOnSameLevel - transform.position).normalized;
                transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
                
                _verticalMovementTimer -= Time.deltaTime;
                if (_verticalMovementTimer <= 0) {
                    _verticalMovement = false;
                    transform.position = targetPosition;
                }
            } else {
                //Horizontal movement logic
                Vector3 targetDirection = (targetPosition - transform.position).normalized;
                //First Rotate
                var angle = Vector3.Angle(transform.forward, targetDirection);
                if (angle > stoppingAngle) {
                    transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * rotationSpeed);
                }
                //Only start to move if the facing direction is < 90º
                if (angle < 90) {
                    transform.position += targetDirection * (moveSpeed * Time.deltaTime);
                }
            }

            if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance) {
                _currentPositionIndex++;
                if (_currentPositionIndex >= _targetPositionsList.Count) {
                    ActionComplete();
                    MoveActionCompleted?.Invoke(this, EventArgs.Empty);
                } else {
                    //Check if the next position is in the same floor
                    var nextGridPosition = LevelGrid.Instance.GetGridPosition(_targetPositionsList[_currentPositionIndex]);
                    var currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                    if (nextGridPosition.FloorNumber != currentGridPosition.FloorNumber) {
                        //Different floor, trigger animation logic
                        _verticalMovement = true;
                        _verticalMovementTimer = _verticalMovementDuration;
                        
                        JumpActionStarted?.Invoke(this, new JumpActionEventArgs {
                            CurrentGridPosition = currentGridPosition,
                            TargetGridPosition = nextGridPosition
                        });
                    }
                }
            }
        }

        private void Move(GridPosition gridPosition) {
            var pathGridPositions = Pathfinding.Instance.FindPath(Unit.GetGridPosition(), gridPosition, out int _);
            _currentPositionIndex = 0;
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