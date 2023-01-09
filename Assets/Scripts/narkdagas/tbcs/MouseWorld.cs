using System;
using UnityEngine;

namespace narkdagas.tbcs {
    public class MouseWorld : MonoBehaviour {

        private static MouseWorld instance;
        
        [SerializeField] private LayerMask mousePlaneLayerMask;

        private void Awake() {
            instance = this;
        }

        void Update() {
            transform.position = MouseWorld.GetPosition();
        }

        public static Vector3 GetPosition() {
            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(screenPointToRay, out var hit, float.MaxValue, instance.mousePlaneLayerMask);
            return hit.point;
        }
    }
}