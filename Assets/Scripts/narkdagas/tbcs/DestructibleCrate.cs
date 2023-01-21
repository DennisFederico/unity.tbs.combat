using System;
using UnityEngine;

namespace narkdagas.tbcs {
    
    public class DestructibleCrate : MonoBehaviour {

        public static event EventHandler onAnyDestroyed;
        [SerializeField] private Transform crateDestroyedPrefab;
        private float _explosionForce = 100f;
        private float _explosionRadius = 10f;
        
        public void Damage(int dmg) {
            var destroyedCrate = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
            ApplyForceToDestroyedCrate(transform.position, destroyedCrate);
            Destroy(gameObject);
            onAnyDestroyed?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyForceToDestroyedCrate(Vector3 forceOrigin, Transform destroyedObject) {
            foreach (Transform child in destroyedObject) {
                if (child.TryGetComponent(out Rigidbody childRigidBody)) {
                    childRigidBody.AddExplosionForce(_explosionForce, forceOrigin, _explosionRadius);
                }
                ApplyForceToDestroyedCrate(forceOrigin, child);
            }
        }
    }
}