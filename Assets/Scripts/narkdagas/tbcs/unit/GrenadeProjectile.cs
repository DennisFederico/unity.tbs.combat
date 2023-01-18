using System;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class GrenadeProjectile : MonoBehaviour {

        private Vector3 _targetPosition;
        private readonly float _moveSpeed = 15f;
        private readonly float _reachedTargetDistance = 0.1f;
        private float _radius;
        private int _damage;
        private Action _onGrenadeCompleteCallback;

        private void Update() {
            if (Vector3.Distance(_targetPosition, transform.position) < _reachedTargetDistance) {
                var colliders = Physics.OverlapSphere(_targetPosition, _radius);
                foreach (var targetCollider in colliders) {
                    if (targetCollider.TryGetComponent(out Unit target)) {
                        //TODO PUSH THE TARGETS
                        target.TakeDamage(_damage);
                    }
                }
                Destroy(gameObject);
                _onGrenadeCompleteCallback();
            }
            Vector3 moveDirection = (_targetPosition - transform.position).normalized;
            transform.position += moveDirection * (_moveSpeed * Time.deltaTime);
        }

        public void ThrowTo(GridPosition targetGridPosition, float radius, int dmg, Action onGrenadeCompleteCallback) {
            _onGrenadeCompleteCallback = onGrenadeCompleteCallback;
            //GridPosition "cellsize" must be taken into account
            _radius = radius * LevelGrid.Instance.GetGridDimension().CellSize;
            _damage = dmg;
            _targetPosition = LevelGrid.Instance.GetGridWorldPosition(targetGridPosition);
        }
    }
}