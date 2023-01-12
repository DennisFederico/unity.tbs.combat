using System;
using UnityEngine;

namespace narkdagas.tbcs {
    public class TurnSystem : MonoBehaviour {
        public static TurnSystem Instance { get; private set; }

        public event EventHandler OnTurnChanged;
        private int _turnNumber = 1;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one TurnSystem in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void NextTurn() {
            _turnNumber++;
            OnTurnChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetCurrentTurn() {
            return _turnNumber;
        }
    }
}