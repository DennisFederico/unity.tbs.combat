using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class UnitRagdollPose : MonoBehaviour {
        [SerializeField] private Transform ragdollRootBone;
        //TODO PHYSICS FROM THE DIRECTION OF THE HIT
        private float force = 300f;
        private float forceRange = 10f;
        
        public void CopyPose(Transform originalRootBone) {
            CloneTransformChildRecursive(originalRootBone, ragdollRootBone);
            Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            ApplyForceToRagdoll(ragdollRootBone, force, transform.position + randomDir, forceRange);
        }

        private void CloneTransformChildRecursive(Transform original, Transform clone) {
            foreach (Transform childTransform in original) {
                Transform cloneChild = clone.Find(childTransform.name);
                if (cloneChild != null) {
                    cloneChild.position = childTransform.position;
                    cloneChild.rotation = childTransform.rotation;
                    CloneTransformChildRecursive(childTransform, cloneChild);
                }
            }
        }
        
        
        private void ApplyForceToRagdoll(Transform root, float force, Vector3 position, float range) {
            foreach (Transform child in root) {
                if (child.TryGetComponent(out Rigidbody childRigidbody)) {
                    childRigidbody.AddExplosionForce(force, position, range);
                }
                ApplyForceToRagdoll(child, force, position, range);
            }            
        }
    }
}