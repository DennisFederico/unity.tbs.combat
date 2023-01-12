using System;
using narkdagas.tbcs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace narkdagas.ui {
    public class TurnSystemUI : MonoBehaviour {

        public static TurnSystemUI Instance { get; private set; }
        
        [SerializeField] private Button endTurnButton;
        [SerializeField] private TextMeshProUGUI textCounter;
        
        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one TurnSystemUI in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start() {
            endTurnButton.onClick.AddListener(() => {
                TurnSystem.Instance.NextTurn();
                UpdateTurnText();
            });
            
            TurnSystem.Instance.OnTurnChanged += (_, _) => UpdateTurnText();
            
            UpdateTurnText();
        }

        private void UpdateTurnText() {
            textCounter.text = $"TURN {TurnSystem.Instance.GetCurrentTurn()}";
        }
    }
}