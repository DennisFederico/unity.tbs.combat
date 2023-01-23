using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridSystemVisualSingle : MonoBehaviour {
        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private GameObject selectedGameObject;

        public void Show(Material material) {
            meshRenderer.material = material;
            meshRenderer.enabled = true;
        }

        public void Hide() {
            meshRenderer.enabled = false;
        }
        
        public void ShowSelected() {
            if (selectedGameObject)
                selectedGameObject.SetActive(true);            
        }
        
        public void HideSelected() {
            if (selectedGameObject)
                selectedGameObject.SetActive(false);            
        }
    }
}