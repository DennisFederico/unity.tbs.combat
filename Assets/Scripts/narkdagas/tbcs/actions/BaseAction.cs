using System;
using System.Collections.Generic;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public abstract class BaseAction : MonoBehaviour {
        //public delegate void OnActionCompleteDelegate();
        
        protected Unit Unit;
        protected bool IsActive;
        protected Action OnActionComplete;
        protected int apCost = 1;

        protected virtual void Awake() {
            Unit = GetComponent<Unit>();
        }

        public abstract string GetActionNameLabel();

        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
        
        
        public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
            return GetValidActionGridPositionList().Contains(gridPosition);
        }

        public virtual int GetAPCost() {
            return 1;
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();
    }
}