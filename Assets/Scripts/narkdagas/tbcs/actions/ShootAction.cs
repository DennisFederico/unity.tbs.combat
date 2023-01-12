using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class ShootAction : BaseAction {
        public override event EventHandler ActionStarted;
        public override event EventHandler ActionCompleted;
        [SerializeField] private int maxShootDistance = 7;
        [SerializeField] private int shootDamage = 2;

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
                    ActionCompleted?.Invoke(this, EventArgs.Empty);
                    return State.None;
                default:
                    return State.None;
            }
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            ActionStart(onActionComplete);
            _currentState = State.Aiming;
            _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            _canShootBullet = true;
            float aimingStateTime = 1f;
            _stateTimer = aimingStateTime;
        }

        private void DoShoot() {
            ActionStarted?.Invoke(this, EventArgs.Empty);
            _targetUnit.TakeDamage(shootDamage);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            var unitGridPosition = Unit.GetGridPosition();

            for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + unitGridPosition;
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        IsInShootingDistance(x, z, maxShootDistance) &&
                        LevelGrid.Instance.IsEnemyAtGridPosition(gridPositionCandidate, Unit.IsEnemyUnit())) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }

            return validGridPositionList;
        }

        private bool IsInShootingDistance(int x, int z, int shootDistance) {
            return Mathf.Sqrt(x * x + z * z) <= shootDistance;
        }

        public override int GetAPCost() {
            return 2;
        }
    }
}