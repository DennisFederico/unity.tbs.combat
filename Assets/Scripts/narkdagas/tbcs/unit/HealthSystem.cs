using System;
using UnityEngine;

namespace narkdagas.tbcs.unit {
    public class HealthSystem : MonoBehaviour {

        public event EventHandler OnDead;
        public event EventHandler OnHealthChanged;
        [SerializeField] private int health = 100;
        private int _healthMax;

        private void Awake() {
            _healthMax = health;
        }

        public void Damage(int damageAmount) {
            if (health <= 0) return;
            health -= damageAmount;
            OnHealthChanged?.Invoke(this, EventArgs.Empty);
            health = health < 0 ? 0 : health;
            if (health == 0) {
                Die();
            }
        }

        private void Die() {
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        public float GetNormalizedHealth() {
            return (float) health / _healthMax;
        }
    }
}