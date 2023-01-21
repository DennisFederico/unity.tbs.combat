using System;
using System.Collections.Generic;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class UnitManager : MonoBehaviour {
        public static UnitManager Instance { get; private set; }
        private List<Unit> _allUnitsList;
        private List<Unit> _enemyUnitsList;
        private List<Unit> _friendlyUnitsList;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one UnitManager in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _allUnitsList = new List<Unit>();
            _enemyUnitsList = new List<Unit>();
            _friendlyUnitsList = new List<Unit>();
        }

        private void Start() {
            Unit.OnAnyUnitSpawn += Unit_OnAnyUnitSpawn;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        void Unit_OnAnyUnitSpawn(object caller, EventArgs args) {
            Unit unit = caller as Unit;
            if (unit == null) return;
            _allUnitsList.Add(unit);
            if (unit.IsEnemyUnit()) {
                _enemyUnitsList.Add(unit);
            } else {
                _friendlyUnitsList.Add(unit);
            }
        }

        void Unit_OnAnyUnitDead(object caller, EventArgs args) {
            Unit unit = caller as Unit;
            if (unit == null) return;
            _allUnitsList.Remove(unit);
            if (unit.IsEnemyUnit()) {
                _enemyUnitsList.Remove(unit);
            } else {
                _friendlyUnitsList.Remove(unit);
            }
        }

        public List<Unit> GetAllUnitsList() {
            return _allUnitsList;
        }
        
        public List<Unit> GetFriendlyUnitsList() {
            return _friendlyUnitsList;
        }
        
        public List<Unit> GetEnemyUnitsList() {
            return _enemyUnitsList;
        }
    }
}