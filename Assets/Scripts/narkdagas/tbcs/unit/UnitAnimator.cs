using System;
using narkdagas.tbcs.actions;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class UnitAnimator : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bulletProjectilePrefab;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform rifleTransform;
        [SerializeField] private Transform swordTransform;

        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
        private static readonly int AnimShoot = Animator.StringToHash("Shoot");
        private static readonly int AnimSlash = Animator.StringToHash("SwordSlash");

        private void Awake() {
            if (TryGetComponent(out MoveAction moveAction)) {
                moveAction.MoveActionStarted += MoveMoveActionMoveActionStarted;
                moveAction.MoveActionCompleted += MoveMoveActionMoveActionCompleted;
            }

            if (TryGetComponent(out ShootAction shootAction)) {
                shootAction.ShootActionStarted += ShootShootActionShootActionStarted;
                shootAction.ShootActionCompleted += ShootShootActionShootActionCompleted;
            }
            
            if (TryGetComponent(out SwordAction swordAction)) {
                swordAction.SwordActionStarted += SwordActionShootActionStarted;
                swordAction.SwordActionCompleted += SwordActionShootActionCompleted;
            }
        }

        private void Start() {
            SwitchToRifle();
        }

        private void MoveMoveActionMoveActionStarted(object sender, EventArgs args) {
            animator.SetBool(AnimIsWalking, true);
        }

        private void MoveMoveActionMoveActionCompleted(object sender, EventArgs args) {
            animator.SetBool(AnimIsWalking, false);
        }

        private void ShootShootActionShootActionStarted(object sender, ShootAction.ShootActionStartedEventArgs args) {
            animator.SetTrigger(AnimShoot);
            var bullet = Instantiate(bulletProjectilePrefab, shootPoint.position, Quaternion.identity);
            var targetPosition = args.TargetUnit.GetWorldPosition();
            targetPosition.y = shootPoint.position.y; // Level to the bullet height
            bullet.GetComponent<BulletProjectile>().SetTarget(targetPosition);
        }

        private void ShootShootActionShootActionCompleted(object sender, EventArgs args) {
            animator.ResetTrigger(AnimShoot);
        }
        
        private void SwordActionShootActionStarted(object sender, SwordAction.SwordActionStartedEventArgs args) {
            SwitchToSword();
            animator.SetTrigger(AnimSlash);
        }
        
        private void SwordActionShootActionCompleted(object sender, EventArgs args) {
            animator.ResetTrigger(AnimSlash);
            SwitchToRifle();
        }

        private void SwitchToSword() {
            swordTransform.gameObject.SetActive(true);
            rifleTransform.gameObject.SetActive(false);
        }

        private void SwitchToRifle() {
            swordTransform.gameObject.SetActive(false);
            rifleTransform.gameObject.SetActive(true);
        }
    }
}