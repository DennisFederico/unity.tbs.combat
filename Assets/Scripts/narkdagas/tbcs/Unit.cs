using System;
using narkdagas.tbcs;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private Animator unitAnimator;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private float stoppingAngle = .1f;
    private Vector3 _targetPosition;
    private Vector3 _targetDirection;
    private Vector3 _initialDirection;
    
    private float _moveStartTime;
    private Vector3 _startPosition;
    private float _moveDistance;
    private float _moveTime;
    private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");

    private void Awake() {
        unitAnimator = GetComponentInChildren<Animator>();
    }

    private void Move(Vector3 targetPos) {
        _targetPosition = targetPos;
        _initialDirection = transform.forward;
        _targetDirection = (_targetPosition - transform.position).normalized;
        
        //face to the direction
        //transform.forward = _targetDirection;
        //Setup movement
        // _moveStartTime = Time.time;
        // _moveDistance = Vector3.Distance(transform.position, targetPos);
        // _moveTime = _moveDistance / moveSpeed;
        // _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        
        //First Rotate
        var angle = Vector3.Angle(transform.forward, _targetDirection);
        if (angle > stoppingAngle) {
            transform.forward = Vector3.Lerp(transform.forward, _targetDirection, Time.deltaTime * 10f);
        }
        
        //Move if far and less than pi/2 (90º)
        if (Vector3.Distance(transform.position, _targetPosition) > stoppingDistance && angle < 90) {
            transform.position += _targetDirection * (moveSpeed * Time.deltaTime);
            //transform.forward = Vector3.Lerp(transform.forward, _targetDirection, Time.deltaTime * 10f);
            // float distCovered = (Time.time - _moveStartTime) * moveSpeed / _moveDistance;
            // float timeProportion = (Time.time - _moveStartTime) / _moveTime;
            // Debug.Log($"Move Time Proportion {timeProportion} for {Time.deltaTime}");
            // transform.position = Vector3.Lerp(_startPosition, _targetPosition, timeProportion);
            unitAnimator.SetBool(AnimIsWalking, true);
        } else {
            unitAnimator.SetBool(AnimIsWalking, false);
        }

        if (Input.GetMouseButtonDown(0)) {
            Move(MouseWorld.GetPosition());
        }
    }
}