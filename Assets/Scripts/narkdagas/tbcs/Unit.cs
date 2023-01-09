using narkdagas.tbcs;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stoppingDistance = .1f;
    private Vector3 _targetPosition;
    private float _moveStartTime;
    private Vector3 _startPosition;
    private float _moveDistance;
    private float _moveTime;

    private void Move(Vector3 targetPos) {
        this._targetPosition = targetPos;
        _moveStartTime = Time.time;
        _moveDistance = Vector3.Distance(transform.position, targetPos);
        _moveTime = _moveDistance / moveSpeed;
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance) {
            float distCovered = (Time.time - _moveStartTime) * moveSpeed / _moveDistance;
            float timeElapsed = (Time.time - _moveStartTime) / _moveTime;
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, timeElapsed);
            //Vector3 moveDir = (_targetPosition - transform.position).normalized;
            //transform.position += moveDir * (moveSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0)) {
            Move(MouseWorld.GetPosition());
        }
    }
}