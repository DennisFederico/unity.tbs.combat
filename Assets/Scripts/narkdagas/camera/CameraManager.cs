using System;
using Cinemachine;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.unit;
using UnityEngine;

namespace narkdagas.camera {
    public class CameraManager : MonoBehaviour {
        public static CameraManager Instance { get; private set; }
        [SerializeField] private GameObject actionCameraGameObject;
        [SerializeField] private CinemachineImpulseSource actionCameraImpulseSource;
        private EventHandler _onAnyActionStartedAction;
        private EventHandler _onAnyActionCompletedAction;

        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one CameraManager in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            actionCameraImpulseSource = GetComponent<CinemachineImpulseSource>();
        }

        private void Start() {
            _onAnyActionStartedAction = (sender, args) => {
                switch (sender) {
                    case ShootAction shootAction:
                        Unit shooterUnit = ((ShootAction.ShootActionStartedEventArgs)args).ShootingUnit;
                        Unit targetUnit = ((ShootAction.ShootActionStartedEventArgs)args).TargetUnit;
                        var shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                        //From where to Look
                        Vector3 cameraHeight = Vector3.up * 1.67f;
                        Vector3 cameraRight = Quaternion.Euler(0, 90, 0) * shootDirection * 0.3f;
                        Vector3 cameraBack = shootDirection * -1;
                        var cameraPosition = shooterUnit.GetWorldPosition() + cameraHeight + cameraRight + cameraBack;
                        actionCameraGameObject.transform.position = cameraPosition;
                        actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraHeight);
                        ShowActionCamera();
                        break;
                }
            };
            BaseAction.OnAnyActionStarted += _onAnyActionStartedAction;

            _onAnyActionCompletedAction = (sender, _) => {
                switch (sender) {
                    case ShootAction shootAction:
                        HideActionCamera();
                        break;
                }
            };
            BaseAction.OnAnyActionCompleted += _onAnyActionCompletedAction;
            ShootAction.OnAnyShootAction += (_,_) => ShakeCamera();

            HideActionCamera();
        }

        private void ShowActionCamera() {
            actionCameraGameObject.SetActive(true);
        }

        private void HideActionCamera() {
            actionCameraGameObject.SetActive(false);
        }

        public void ShakeCamera(float intensity = 1f) {
            actionCameraImpulseSource.GenerateImpulse(intensity);
        }
    }
}