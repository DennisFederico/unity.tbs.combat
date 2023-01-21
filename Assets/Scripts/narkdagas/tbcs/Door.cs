using System;
using System.Collections;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs {
    //TODO extend from Interactable
    public class Door : MonoBehaviour {
        private GridPosition _currentGridPosition;
        private Animator _animator;
        private static readonly int AnimIsOpen = Animator.StringToHash("IsOpen");
        [SerializeField] private bool isOpen;
        private Action _onInteractionComplete;
        private bool _isActive;
        private float _timer;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            _currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetDoorAtGridPosition(_currentGridPosition, this);
            Interact(isOpen);
        }

        public void Interact(Action onInteractionComplete) {
            //if (!_isInteractable) return;
            _onInteractionComplete = onInteractionComplete;
            _isActive = true;
            _timer = .5f;
            Interact(!isOpen);
            //StartCoroutine(DelayCallback(onComplete, 1f));
        }

        private void Interact(bool open) {
            isOpen = open;
            _animator.SetBool(AnimIsOpen, isOpen);
            Pathfinding.Instance.SetIsPositionWalkable(_currentGridPosition, isOpen);
        }

        private void Update() {
            if (!_isActive) return;
            
            _timer -= Time.deltaTime;
            if (_timer < 0) {
                _isActive = false;
                _onInteractionComplete();
            }
        }

        // IEnumerator DelayCallback(Action callback, float delaySeconds) {
        //     Debug.Log("Delay Started");
        //     yield return new WaitForSeconds(delaySeconds);
        //     Debug.Log("elapsed!!");
        //     callback();
        // }
    }
}