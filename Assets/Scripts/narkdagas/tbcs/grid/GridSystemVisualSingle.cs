using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridSystemVisualSingle : MonoBehaviour {
        
        [SerializeField] private MeshRenderer meshRenderer;

        public void Show(Material material) {
            meshRenderer.material = material;
            meshRenderer.enabled = true;
        }

        public void Hide() {
            meshRenderer.enabled = false;
        }
    }
}