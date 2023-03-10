using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class SpinAction : BaseAction {

        private float _totalSpin;

        private void Update() {
            if (!IsActive) return;
            float spinAmount = 360f * Time.deltaTime;
            //transform.eulerAngles += new Vector3(0, spinAmount, 0);
            transform.Rotate(Vector3.up, spinAmount);
            _totalSpin += spinAmount;
            if (_totalSpin >= 360) {
                ActionComplete();
            }
        }

        private void Spin() {
            _totalSpin = 0f;
        }

        public override string GetActionNameLabel() {
            return "Spin";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
            Spin();
            ActionStart(onActionComplete, EventArgs.Empty);
        }

        public override List<GridPosition> GetValidActionGridPositionList() {
            GridPosition unitGridPosition = Unit.GetGridPosition();
            return new List<GridPosition>() { unitGridPosition };
        }

        public override int GetAPCost() {
            return 1;
        }

        public override EnemyAIActionData GetEnemyAIActionData(GridPosition gridPosition) {
            return new EnemyAIActionData {
                GridPosition = gridPosition,
                ActionValue = 0
            };
        }
    }
}