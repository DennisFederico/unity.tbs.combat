using TMPro;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridDebugObject : MonoBehaviour {
        private TextMeshPro _text;
        private GridObject _gridObject;

        private void Awake() {
            _text = GetComponentInChildren<TextMeshPro>();
        }
        
        private void Update() {
            _text.text = _gridObject.ToString();
        }

        public void SetGridObject(GridObject gridObject) {
            _gridObject = gridObject;
        }
    }
}