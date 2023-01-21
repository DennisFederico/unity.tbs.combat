using System;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.systems {
    public class EnemyAI : MonoBehaviour {

        private enum State {
            WaitForEnemyTurn,
            TakingTurn,
            Busy,
        }

        private State _currentState;
        private float _timer;

        private void Awake() {
            _currentState = State.WaitForEnemyTurn;
        }

        private void Start() {
            TurnSystem.Instance.OnTurnChanged += (_, isPlayerTurn) => {
                if (!isPlayerTurn) {
                    _currentState = State.TakingTurn;
                    _timer = 2f;
                }
            };
        }

        private void Update() {
            if (TurnSystem.Instance.IsPlayerTurn()) return;

            switch (_currentState) {
                case State.WaitForEnemyTurn:
                    break;
                case State.TakingTurn:
                    _timer -= Time.deltaTime;
                    if (_timer <= 0f) {
                        if (TryTakeAction(SetStateTakingTurn)) {
                            _currentState = State.Busy;    
                        } else {
                            TurnSystem.Instance.NextTurn();    
                        }
                    }
                    break;
                case State.Busy:
                    break;
            }
        }

        private void SetStateTakingTurn() {
            _timer = .5f;
            _currentState = State.TakingTurn;
        }

        private bool TryTakeAction(Action onActionComplete) {
            foreach (Unit unit in UnitManager.Instance.GetEnemyUnitsList()) {
                if (TryTakeAIUnitAction(unit, onActionComplete)) return true;
            }
            return false;
        }

        private bool TryTakeAIUnitAction(Unit unit, Action onActionComplete) {

            EnemyAIActionData bestActionData = null;
            BaseAction action = null;
            foreach (BaseAction baseAction in unit.GetActions()) {
                if (!unit.CanSpendActionPoints(baseAction)) continue;
                var actionData = baseAction.GetBestEnemyAIAction();
                if (bestActionData == null) {
                    bestActionData = actionData;
                    action = baseAction;
                } else if (actionData != null && actionData.ActionValue > bestActionData.ActionValue) {
                    bestActionData = actionData;
                    action = baseAction;
                }
            }

            if (bestActionData != null && unit.TrySpendActionPoints(action)) {
                action.TakeAction(bestActionData.GridPosition, onActionComplete);
                return true;
            }
            return false;
        }
    }
}