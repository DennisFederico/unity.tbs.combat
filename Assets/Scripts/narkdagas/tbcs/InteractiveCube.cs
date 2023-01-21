using System;
using System.Collections;
using narkdagas.tbcs.actions;
using narkdagas.tbcs.grid;
using UnityEngine;

namespace narkdagas.tbcs {
    public class InteractiveCube : MonoBehaviour, IInteractable {
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        private bool _isGreen;
        
        private void Start() {
            var currentGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetInteractableAtGridPosition(currentGridPosition, this);
            SetRed();
            _isGreen = false;
        }

        private void SetGreen() {
            meshRenderer.material = greenMaterial;
            _isGreen = true;
        }
        
        private void SetRed() {
            meshRenderer.material = redMaterial;
            _isGreen = false;
        }

        private void SwitchMaterial() {
            _isGreen = !_isGreen;
            meshRenderer.material = _isGreen ? greenMaterial : redMaterial;
        }

        public void Interact(Action onInteractionComplete) {
            SwitchMaterial();
            StartCoroutine(DelayCallback(onInteractionComplete, 0.5f));
        }
        
        IEnumerator DelayCallback(Action callback, float delaySeconds) {
            yield return new WaitForSeconds(delaySeconds);
            callback();
        }
    }
}