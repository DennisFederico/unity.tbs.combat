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

        private Vector3 _targetPosition;
        private Vector3 _targetDirection;

        protected override void Awake() {
            base.Awake();
            _targetDirection = transform.forward;
            _targetPosition = transform.position;
        }

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
            //First Rotate
            var angle = Vector3.Angle(transform.forward, _targetDirection);
            if (angle > stoppingAngle) {
                transform.forward = Vector3.Lerp(transform.forward, _targetDirection, Time.deltaTime * rotationSpeed);
            }

            //Only start to move if the facing direction is < 90º
            if (angle < 90) {
                if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance) {
                    transform.position += _targetDirection * (moveSpeed * Time.deltaTime);
                } else {
                    ActionComplete();
                    MoveActionCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Move(GridPosition gridPosition) {
            _targetPosition = LevelGrid.Instance.GetGridWorldPosition(gridPosition);
            _targetDirection = (_targetPosition - transform.position).normalized;
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            var unitGridPosition = Unit.GetGridPosition();

            for (int x = -maxMoveGridDistance; x <= maxMoveGridDistance; x++) {
                for (int z = -maxMoveGridDistance; z <= maxMoveGridDistance; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + unitGridPosition;
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        LevelGrid.Instance.IsGridPositionFree(gridPositionCandidate)) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }

            return validGridPositionList;
        }
        
        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            var targetCount = Unit.GetShootAction().GetTargetCountAtGridPosition(gridPosition);
            
            return new EnemyAIActionData {
                GridPosition = gridPosition,
                ActionValue = targetCount * 10
            };
        }
    }
}