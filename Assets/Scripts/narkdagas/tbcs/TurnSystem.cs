using System;
using UnityEngine;

namespace narkdagas.tbcs {
    public class TurnSystem : MonoBehaviour {
        public static TurnSystem Instance { get; private set; }
        public event EventHandler<bool> OnTurnChanged;
        private int _turnNumber = 1;
        private bool _isPlayerTurn = true;

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
            _isPlayerTurn = !_isPlayerTurn;
            OnTurnChanged?.Invoke(this, _isPlayerTurn);
        }

        public int GetCurrentTurn() {
            return _turnNumber;
        }

        public bool IsPlayerTurn() {
            return _isPlayerTurn;
        }
    }
}