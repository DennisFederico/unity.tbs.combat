using System;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public abstract class BaseAction : MonoBehaviour {
        //public delegate void OnActionCompleteDelegate();
        
        protected Unit Unit;
        protected bool IsActive;
        protected Action OnActionComplete;

        protected virtual void Awake() {
            Unit = GetComponent<Unit>();
        }

        public abstract string GetActionNameLabel();
    }
}