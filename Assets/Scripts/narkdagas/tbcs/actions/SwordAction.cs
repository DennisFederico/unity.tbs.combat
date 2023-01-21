using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class SwordAction : BaseAction {
        public static event EventHandler OnAnySwordHit; 
        public event EventHandler<SwordActionStartedEventArgs> SwordActionStarted;
        public event EventHandler SwordActionCompleted;
        public class SwordActionStartedEventArgs : EventArgs {
            public Unit TargetUnit;
            public Unit SwordUnit;
        }
        
        [SerializeField] private int maxSwordDistance = 1;
        [SerializeField] private int damage = 100;
        

        private Unit _targetUnit;
        private State _currentState;
        private float _stateTimer;
        private enum State {
            SwingBeforeHit,
            SwingAfterHit,
            None
        }
        
        public override string GetActionNameLabel() {
            return "Sword";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            _currentState = State.SwingBeforeHit;
            float beforeHitStateTime = 0.7f;
            _stateTimer = beforeHitStateTime;
            _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            SwordActionStarted?.Invoke(this, new SwordActionStartedEventArgs() {
                TargetUnit = _targetUnit,
                SwordUnit = Unit
            });
            ActionStart(onActionComplete, EventArgs.Empty);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            
            for (int x = -maxSwordDistance; x <= maxSwordDistance; x++) {
                for (int z = -maxSwordDistance; z <= maxSwordDistance; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + Unit.GetGridPosition();
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        LevelGrid.Instance.IsEnemyAtGridPosition(gridPositionCandidate, Unit.IsEnemyUnit())) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            return validGridPositionList;
        }

        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            return new EnemyAIActionData() {
                GridPosition = gridPosition,
                ActionValue = 200
            };
        }

        public int GetSwordActionRange() {
            return maxSwordDistance;
        }
        
        private void Update() {
            if (!IsActive) return;
            if (_currentState == State.None) return;

            _stateTimer -= Time.deltaTime;
            switch (_currentState) {
                case State.SwingBeforeHit:
                    Vector3 aimDir = (_targetUnit.GetWorldPosition() - transform.position).normalized;
                    float rotateSpeed = 10f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                    break;
                case State.SwingAfterHit:
                    break;
            }

            if (_currentState != State.None && _stateTimer <= 0f) {
                _currentState = TransitionToNextState(_currentState);
            }
        }
        
        private State TransitionToNextState(State fromState) {
            switch (fromState) {
                case State.SwingBeforeHit:
                    float afterHitStateTime = 0.5f;
                    _stateTimer = afterHitStateTime;
                    _targetUnit.TakeDamage(damage);
                    OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                    return State.SwingAfterHit;
                case State.SwingAfterHit:
                    SwordActionCompleted?.Invoke(this, EventArgs.Empty);
                    ActionComplete();
                    return State.None;
                default:
                    return State.None;
            }
        }
    }
}