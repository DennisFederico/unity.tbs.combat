using System;
using UnityEngine;

namespace narkdagas.tbcs {
    public class EnemyAI : MonoBehaviour {
        
        private float _timer;

        private void Start() {
            TurnSystem.Instance.OnTurnChanged += (_, isPlayerTurn) => {
                if (!isPlayerTurn) _timer = 3;
            };
        }

        private void Update() {
            if (TurnSystem.Instance.IsPlayerTurn()) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0f) {
                TurnSystem.Instance.NextTurn();
            }
        }
    }
}