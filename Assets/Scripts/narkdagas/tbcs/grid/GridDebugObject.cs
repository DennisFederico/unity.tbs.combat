using TMPro;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridDebugObject : MonoBehaviour {

        [SerializeField] private TextMeshPro gridObjectText;
        private object _gridObject;

        protected virtual void Update() {
            gridObjectText.text = _gridObject.ToString();
        }

        public virtual void SetGridObject(object gridObject) {
            _gridObject = gridObject;
        }
    }
}