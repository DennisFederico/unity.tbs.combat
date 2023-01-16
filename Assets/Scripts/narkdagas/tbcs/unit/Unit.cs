using System;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.grid;
using UnityEngine;
using UnityEngine.Serialization;

namespace narkdagas.tbcs.unit {
    public class Unit : MonoBehaviour {

        public static event EventHandler OnAnyUnitSpawn;
        public static event EventHandler OnAnyUnitDead; 

        [FormerlySerializedAs("isEnemy")] [SerializeField]
        private bool isEnemyUnit;
        
        public event EventHandler OnActionPointsChanged;

        [SerializeField] private int maxActionPoints = 2;
        private GridPosition _currentGridPosition;
        private HealthSystem _healthSystem;
        private BaseAction[] _baseActions;
        private int _actionPoints;

        private void Awake() {
            _healthSystem = GetComponent<HealthSystem>();
            _baseActions = GetComponents<BaseAction>();
            _actionPoints = maxActionPoints;
        }

        private void Start() {
            _currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(_currentGridPosition, this);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            _healthSystem.OnDead += HealthSystem_OnDead;
            OnAnyUnitSpawn?.Invoke(this, EventArgs.Empty);
        }

        // Update is called once per frame
        void Update() {
            var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != _currentGridPosition) {
                var oldGridPosition = _currentGridPosition;
                _currentGridPosition = newGridPosition;
                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }
        
        public BaseAction[] GetActions() {
            return _baseActions;
        }

        public T GetAction<T>() where T : BaseAction {
            foreach (var baseAction in _baseActions) {
                if (baseAction is T action) return action;
            }
            return null;
        }

        public GridPosition GetGridPosition() {
            return _currentGridPosition;
        }

        public Vector3 GetWorldPosition() {
            return transform.position;
        }

        public bool TrySpendActionPoints(BaseAction baseAction) {
            if (CanSpendActionPoints(baseAction)) {
                SpendActionPoints(baseAction.GetAPCost());
                return true;
            }

            return false;
        }

        public bool CanSpendActionPoints(BaseAction action) {
            return _actionPoints >= action.GetAPCost();
        }

        private void SpendActionPoints(int amount) {
            _actionPoints -= amount;
            OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints() {
            return _actionPoints;
        }

        public bool IsEnemyUnit() {
            return isEnemyUnit;
        }

        public void TakeDamage(int damage) {
            _healthSystem.Damage(damage);
        }

        public float GetHealthNormalized() {
            return _healthSystem.GetNormalizedHealth();
        }

        private void TurnSystem_OnTurnChanged(object caller, bool isPlayerTurn) {
            if ((isEnemyUnit && !isPlayerTurn) || (isPlayerTurn && !isEnemyUnit)) {
                _actionPoints = maxActionPoints;
                OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HealthSystem_OnDead(object caller, EventArgs args) {
            LevelGrid.Instance.RemoveUnitAtGridPosition(_currentGridPosition, this);
            OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }

        public override string ToString() {
            return transform.gameObject.name;
        }
    }
}