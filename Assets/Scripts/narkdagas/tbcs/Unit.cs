using narkdagas.tbcs.actions;
using narkdagas.tbcs.grid;
using UnityEngine;
using UnityEngine.Serialization;

namespace narkdagas.tbcs {
    public class Unit : MonoBehaviour {

        [FormerlySerializedAs("isEnemy")] [SerializeField] private bool isEnemyUnit;
        [SerializeField] private int maxActionPoints = 2;
        private GridPosition _currentGridPosition;
        private MoveAction _moveAction;
        private SpinAction _spinAction;
        private BaseAction[] _baseActions;
        private int _actionPoints;

        private void Awake() {
            _moveAction = GetComponent<MoveAction>();
            _spinAction = GetComponent<SpinAction>();
            _baseActions = GetComponents<BaseAction>();
            _actionPoints = maxActionPoints;
        }

        private void Start() {
            _currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(_currentGridPosition, this);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        // Update is called once per frame
        void Update() {
            var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != _currentGridPosition) {
                LevelGrid.Instance.UnitMovedGridPosition(this, _currentGridPosition, newGridPosition);
                _currentGridPosition = newGridPosition;
            }
        }

        public BaseAction[] GetActions() {
            return _baseActions;
        }

        public MoveAction GetMoveAction() {
            return _moveAction;
        }
        
        public SpinAction GetSpinAction() {
            return _spinAction;
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
        }

        public int GetActionPoints() {
            return _actionPoints;
        }

        public bool IsEnemyUnit() {
            return isEnemyUnit;
        }

        public void TakeDamage(int damage) {
            Debug.Log($"{this} taking {damage} units of damage");
        }
        
        private void TurnSystem_OnTurnChanged(object caller, bool isPlayerTurn) {
            if ((isEnemyUnit && !isPlayerTurn) || (isPlayerTurn && !isEnemyUnit)) {
                _actionPoints = maxActionPoints;   
            }
        }
        
        public override string ToString() {
            return transform.gameObject.name;
        }
    }
}