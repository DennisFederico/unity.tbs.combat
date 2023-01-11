using System;
using UnityEngine;

namespace narkdagas.tbcs.actions {
    public class SpinAction : MonoBehaviour {

        private bool _spin;
        private void Update() {
            if (_spin) {
                float spinAmount = 360f * Time.deltaTime;
                //transform.eulerAngles += new Vector3(0, spinAmount, 0);
                transform.Rotate(Vector3.up, spinAmount);
            }
        }

        public void Spin() {
            _spin = true;
        }
        
        public void ToggleSpin() {
            _spin = !_spin;
        }
    }
}