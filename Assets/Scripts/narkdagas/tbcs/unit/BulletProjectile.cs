using System;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class BulletProjectile : MonoBehaviour {

        [SerializeField] private Transform bulletHitFxPrefab;
        private Vector3 _targetPosition;
        private bool _move;
        private const float BulletSpeed = 200f;

        private void Update() {
            if (!_move) return;
            Vector3 position = transform.position;
            float distanceBefore = Vector3.Distance(position, _targetPosition);
            Vector3 moveDir = (_targetPosition - position).normalized;
            transform.position += moveDir * (BulletSpeed * Time.deltaTime);
            float distanceAfter = Vector3.Distance(transform.position, _targetPosition);
            if (distanceBefore < distanceAfter) {
                _move = false;
                transform.position = _targetPosition;
                Destroy(gameObject, 1f);
                Instantiate(bulletHitFxPrefab, _targetPosition, Quaternion.identity);
            }
        }

        public void SetTarget(Vector3 targetPosition) {
            _move = true;
            _targetPosition = targetPosition;
        }
    }
}