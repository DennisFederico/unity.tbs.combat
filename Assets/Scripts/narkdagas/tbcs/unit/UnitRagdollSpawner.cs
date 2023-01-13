using System;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class UnitRagdollSpawner : MonoBehaviour {
        [SerializeField] private Transform ragdollPrefab;
        [SerializeField] private Transform unitRootBone;
        private HealthSystem _healthSystem;
        private EventHandler _onDeadAction;

        private void Awake() {
            _healthSystem = GetComponent<HealthSystem>();
            _onDeadAction = delegate { SpawnRagdoll(); };
            _healthSystem.OnDead += _onDeadAction;
        }

        private void SpawnRagdoll() {
            var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            ragdoll.GetComponent<UnitRagdollPose>().CopyPose(unitRootBone);
            _healthSystem.OnDead -= _onDeadAction;
        }
    }
}