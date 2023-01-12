using System;
using narkdagas.tbcs.actions;
using UnityEngine;

namespace narkdagas.tbcs {
    public class UnitAnimator : MonoBehaviour {
        [SerializeField] private Animator animator;

        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
        private static readonly int AnimShoot = Animator.StringToHash("Shoot");
        
        private void Awake() {
            if (TryGetComponent(out MoveAction moveAction)) {
                moveAction.ActionStarted += MoveAction_ActionStarted;
                moveAction.ActionCompleted += MoveAction_ActionCompleted;
            }
            if (TryGetComponent(out ShootAction shootAction)) {
                shootAction.ActionStarted += ShootAction_ActionStarted;
                shootAction.ActionCompleted += ShootAction_ActionCompleted;
            }
        }

        private void MoveAction_ActionStarted(object sender, EventArgs args) {
            animator.SetBool(AnimIsWalking, true);
        }
        private void MoveAction_ActionCompleted(object sender, EventArgs args) {
            animator.SetBool(AnimIsWalking, false);
        }
        
        private void ShootAction_ActionStarted(object sender, EventArgs args) {
            animator.SetTrigger(AnimShoot);
        }
        private void ShootAction_ActionCompleted(object sender, EventArgs args) {
            animator.ResetTrigger(AnimShoot);
        }
    }
}