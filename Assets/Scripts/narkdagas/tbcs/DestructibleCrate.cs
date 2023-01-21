using System;
using UnityEngine;

namespace narkdagas.tbcs {
    
    public class DestructibleCrate : MonoBehaviour {

        public static event EventHandler onAnyDestroyed;
        
        public void Damage(int dmg) {
            Destroy(gameObject);
            onAnyDestroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}