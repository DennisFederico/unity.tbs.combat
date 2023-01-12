using System.Collections;
using UnityEngine;

namespace narkdagas {
  
    public class DestroyOnTimer : MonoBehaviour {
        [SerializeField] private int seconds = 15;
        
        private void Start() {
            StartCoroutine(Destroy());
        }

        IEnumerator Destroy() {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}