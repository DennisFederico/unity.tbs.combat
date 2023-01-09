using UnityEngine;

namespace narkdagas.tbcs {
    public class Unit : MonoBehaviour {

        [SerializeField] private Animator unitAnimator;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float stoppingDistance = .1f;
        [SerializeField] private float stoppingAngle = .1f;
        private Vector3 _targetPosition;
        private Vector3 _targetDirection;
    
        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");

        private void Awake() {
            unitAnimator = GetComponentInChildren<Animator>();
            _targetDirection = transform.forward;
            _targetPosition = transform.position;
        }

        internal void Move(Vector3 targetPos) {
            _targetPosition = targetPos;
            _targetDirection = (_targetPosition - transform.position).normalized;
        }

        // Update is called once per frame
        void Update() {
        
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
            }
        }
    }
}