using System;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridSystemVisualSingle : MonoBehaviour {
        //PENDING: EITHER DISABLE THE WHOLE CHILD HIERARCHY OR GET A REFERENCE OF THE RENDERER

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}