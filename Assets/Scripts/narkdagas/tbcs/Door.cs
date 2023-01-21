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
        private readonly float _actionDuration = 0.75f;

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            _currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetDoorAtGridPosition(_currentGridPosition, this);
            Interact(isOpen);
        }

        public void Interact(Action onInteractionComplete) {
            Interact(!isOpen);
            StartCoroutine(DelayCallback(onInteractionComplete, _actionDuration));
        }

        private void Interact(bool open) {
            isOpen = open;
            _animator.SetBool(AnimIsOpen, isOpen);
            Pathfinding.Instance.SetIsPositionWalkable(_currentGridPosition, isOpen);
        }

        //OPTIONALLY YOU CAN USE A TIMER SET ON INTERACT AND CLOCK IN UPDATE
        IEnumerator DelayCallback(Action callback, float delaySeconds) {
            yield return new WaitForSeconds(delaySeconds);
            callback();
        }
    }
}