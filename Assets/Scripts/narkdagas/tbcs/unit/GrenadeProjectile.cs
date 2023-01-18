using System;
using narkdagas.tbcs.grid;
using Unity.Mathematics;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class GrenadeProjectile : MonoBehaviour {

        public static EventHandler OnAnyGrenadeExplode;
        [SerializeField] private Transform grenadeExplodeFxPrefab;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private AnimationCurve arcYAnimationCurve;
        private Action _onGrenadeCompleteCallback;
        private float _radius;
        private int _damage;
        private Vector3 _targetPosition;
        private readonly float _moveSpeed = 15f;
        private readonly float _reachedTargetDistance = 0.1f;
        private readonly float _maxHeight = 4f;
        private float _heightProportion;
        private float _totalDistance;
        private Vector3 _xzPosition;

        private void Update() {
            if (Vector3.Distance(_targetPosition, _xzPosition) < _reachedTargetDistance) {
                var colliders = Physics.OverlapSphere(_targetPosition, _radius);
                foreach (var targetCollider in colliders) {
                    if (targetCollider.TryGetComponent(out Unit target)) {
                        //TODO PUSH THE TARGETS
                        target.TakeDamage(_damage);
                    }
                }
                OnAnyGrenadeExplode.Invoke(this, EventArgs.Empty);
                Instantiate(grenadeExplodeFxPrefab, _targetPosition + Vector3.up, quaternion.identity);
                //UnParent the trail before destroying the object to keep the trail until the end
                trailRenderer.transform.parent = null;
                Destroy(gameObject);
                _onGrenadeCompleteCallback();
            }
            Vector3 moveDirection = (_targetPosition - _xzPosition).normalized;
            _xzPosition += moveDirection * (_moveSpeed * Time.deltaTime);
            float normalizedDistance = 1 - (Vector3.Distance(_xzPosition, _targetPosition) / _totalDistance);
            
            //Make height proportionate to the distance to avoid step curves when throwing close
            var y = arcYAnimationCurve.Evaluate(normalizedDistance) * _heightProportion;
            transform.position = new Vector3(_xzPosition.x, y, _xzPosition.z);
        }

        public void ThrowTo(GridPosition targetGridPosition, float radius, int dmg, Action onGrenadeCompleteCallback) {
            _targetPosition = LevelGrid.Instance.GetGridWorldPosition(targetGridPosition);
            _xzPosition = transform.position;
            _xzPosition.y = 0;
            _totalDistance = Vector3.Distance(_xzPosition , _targetPosition);
            //TODO this could use refinement, assuming a max distance of 7
            _heightProportion = _totalDistance > 4.5 ? _maxHeight : _totalDistance > 3.5 ? _maxHeight / 2 : _maxHeight / 4;
            _onGrenadeCompleteCallback = onGrenadeCompleteCallback;
            //GridPosition "cellsize" must be taken into account
            _radius = radius * LevelGrid.Instance.GetGridDimension().CellSize;
            _damage = dmg;
        }
    }
}