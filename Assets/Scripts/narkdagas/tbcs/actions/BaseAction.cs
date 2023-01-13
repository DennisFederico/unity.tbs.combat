using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public abstract class BaseAction : MonoBehaviour {
        public static event EventHandler OnAnyActionStarted;
        public static event EventHandler OnAnyActionCompleted;
        protected Unit Unit;
        protected bool IsActive;
        protected Action OnActionComplete;
        protected int apCost = 1;

        protected virtual void Awake() {
            Unit = GetComponent<Unit>();
        }

        public abstract string GetActionNameLabel();

        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

        protected void ActionStart(Action onActionComplete, EventArgs eventArgs) {
            IsActive = true;
            OnActionComplete = onActionComplete;
            OnAnyActionStarted?.Invoke(this, eventArgs);
        }

        protected void ActionComplete() {
            IsActive = false;
            OnActionComplete();
            OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
            return GetValidActionGridPositionList().Contains(gridPosition);
        }

        public virtual int GetAPCost() {
            return 1;
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();
    }
}