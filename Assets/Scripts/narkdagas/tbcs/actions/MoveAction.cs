using System.Collections.Generic;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class MoveAction : MonoBehaviour {
        [SerializeField] private Animator unitAnimator;
        [SerializeField] private int maxMoveGridDistance = 4;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float stoppingAngle = .1f;
        private bool _isMoving;
        private Vector3 _targetPosition;
        private Vector3 _targetDirection;
        private Unit _unit;

        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");

        private void Awake() {
            _targetDirection = transform.forward;
            _targetPosition = transform.position;
            unitAnimator = GetComponentInChildren<Animator>();
            _unit = GetComponent<Unit>();
        }

        void Update() {
            if (_isMoving) {
                //First Rotate
                var angle = Vector3.Angle(transform.forward, _targetDirection);
                if (angle > stoppingAngle) {
                    transform.forward = Vector3.Lerp(transform.forward, _targetDirection, Time.deltaTime * rotationSpeed);
                }

                //Move if far and less than pi/2 (90º)
                if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance && angle < 90) {
                    transform.position += _targetDirection * (moveSpeed * Time.deltaTime);
                    unitAnimator.SetBool(AnimIsWalking, true);
                } else {
                    unitAnimator.SetBool(AnimIsWalking, false);
                    _isMoving = false;
                }
            }
        }

        public void Move(GridPosition gridPosition) {
            _targetPosition = LevelGrid.Instance.GetGridWorldPosition(gridPosition);
            _targetDirection = (_targetPosition - transform.position).normalized;
            _isMoving = true;
        }

        public bool IsValidActionGridPosition(GridPosition gridPosition) {
            return GetValidActionGridPositionList().Contains(gridPosition);
        }

        public List<GridPosition> GetValidActionGridPositionList() {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            var unitGridPosition = _unit.GetGridPosition();

            for (int x = -maxMoveGridDistance; x <= maxMoveGridDistance; x++) {
                for (int z = -maxMoveGridDistance; z <= maxMoveGridDistance; z++) {
                    GridPosition gridPositionCandidate = new GridPosition(x, z) + unitGridPosition;
                    if (LevelGrid.Instance.IsValidGridPosition(gridPositionCandidate) &&
                        LevelGrid.Instance.IsGridPositionFree(gridPositionCandidate)) {
                        validGridPositionList.Add(gridPositionCandidate);
                    }
                }
            }

            return validGridPositionList;
        }
    }
}