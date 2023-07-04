using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class ShootAction : BaseAction {
        public static event EventHandler<ShootActionStartedEventArgs> OnAnyShootAction;
        public event EventHandler<ShootActionStartedEventArgs> ShootActionStarted;
        public event EventHandler ShootActionCompleted;

        public class ShootActionStartedEventArgs : EventArgs {
            public Unit TargetUnit;
            public Unit ShootingUnit;
        }

        //TODO IMPLEMENT AMMO COUNT
        [SerializeField] private int maxShootDistance = 7;
        [SerializeField] private int shootDamage = 20;
        [SerializeField] private LayerMask obstacleLayerMask;

        private enum State {
            Aiming,
            Shooting,
            Cooldown,
            None
        }

        private State _currentState;
        private float _stateTimer;
        private Unit _targetUnit;
        private bool _canShootBullet;
        private static readonly Vector3 ShoulderHeight = Vector3.up * 1.67f;

        public override string GetActionNameLabel() {
            return "Shoot";
        }

        private void Update() {
            if (!IsActive) return;
            if (_currentState == State.None) return;

            _stateTimer -= Time.deltaTime;
            switch (_currentState) {
                case State.Aiming:
                    Vector3 aimDir = (_targetUnit.GetWorldPosition() - transform.position).normalized;
                    float rotateSpeed = 10f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                    break;
                case State.Shooting:
                    if (_canShootBullet) {
                        DoShoot();
                        _canShootBullet = false;
                    }

                    break;
                case State.Cooldown:
                    break;
            }

            if (_currentState != State.None && _stateTimer <= 0f) {
                _currentState = TransitionToNextState(_currentState);
            }
        }

        private State TransitionToNextState(State fromState) {
            switch (fromState) {
                case State.Aiming:
                    float shootingStateTime = 0.5f;
                    _stateTimer = shootingStateTime;
                    return State.Shooting;
                case State.Shooting:
                    _currentState = State.Cooldown;
                    float coolOffSateTime = 0.4f;
                    _stateTimer = coolOffSateTime;
                    return State.Cooldown;
                case State.Cooldown:
                    ActionComplete();
                    ShootActionCompleted?.Invoke(this, EventArgs.Empty);
                    return State.None;
                default:
                    return State.None;
            }
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            _currentState = State.Aiming;
            _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            _canShootBullet = true;
            float aimingStateTime = 1f;
            _stateTimer = aimingStateTime;
            ActionStart(onActionComplete, new ShootActionStartedEventArgs() {
                TargetUnit = _targetUnit,
                ShootingUnit = transform.GetComponent<Unit>()
            });
        }

        private void DoShoot() {
            var shootActionStartedEventArgs = new ShootActionStartedEventArgs() {
                TargetUnit = _targetUnit,
                ShootingUnit = transform.GetComponent<Unit>()
            };
            OnAnyShootAction?.Invoke(this, shootActionStartedEventArgs);
            ShootActionStarted?.Invoke(this, shootActionStartedEventArgs);
            //TODO WHERE AND HOW SHOULD WE APPLY A FORCE TO THE RAGDOLL IF IT DIES FROM A BULLET?
            _targetUnit.TakeDamage(shootDamage);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            return GetValidActionGridPositionList(Unit.GetGridPosition());
        }

        public List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition) {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                    GridPosition gridPositionCandidate = gridPosition % new GridPosition(x, z, 0);
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        IsInShootingDistance(x, z, maxShootDistance) &&
                        LevelGrid.Instance.IsEnemyAtGridPosition(gridPositionCandidate, Unit.IsEnemyUnit()) &&
                        IsEnemyInSight(gridPositionCandidate)) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }
            return validGridPositionList;
        }

        private bool IsEnemyInSight(GridPosition gridPosition) {
            var unitAtGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (unitAtGridPosition.IsEnemyUnit() == Unit.IsEnemyUnit()) return false;
            
            var targetUnitVector = unitAtGridPosition.GetWorldPosition() + ShoulderHeight;
            var unitFireVector = Unit.GetWorldPosition() + ShoulderHeight;
            var targetDirection = (targetUnitVector - unitFireVector).normalized;

            return !Physics.Raycast(unitFireVector,
                targetDirection,
                Vector3.Distance(unitFireVector, targetUnitVector),
                obstacleLayerMask);
        }

        private bool IsInShootingDistance(int x, int z, int shootDistance) {
            return Mathf.Sqrt(x * x + z * z) <= shootDistance;
        }

        public int GetShootRange() {
            return maxShootDistance;
        }

        public override int GetAPCost() {
            return 1;
        }

        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            var healthNormalized = targetUnit.GetHealthNormalized();

            return new EnemyAIActionData {
                GridPosition = gridPosition,
                ActionValue = 100 + Mathf.RoundToInt((1 - healthNormalized) * 100f)
            };
        }

        public int GetTargetCountAtGridPosition(GridPosition gridPosition) {
            return GetValidActionGridPositionList(gridPosition).Count;
        }
    }
}