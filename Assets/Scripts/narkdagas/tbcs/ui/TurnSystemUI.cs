using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace narkdagas.tbcs.ui {
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
                //UpdateTurnText(TurnSystem.Instance.IsPlayerTurn());
            });
            TurnSystem.Instance.OnTurnChanged += (_, isPlayerTurn) => UpdateTurnText(isPlayerTurn);
            TurnSystem.Instance.OnTurnChanged += (_, isPlayerTurn) => UpdateEndTurnButtonVisibility(isPlayerTurn);
            UpdateTurnText(TurnSystem.Instance.IsPlayerTurn());
        }

        private void UpdateTurnText(bool isPlayerTurn) {
            string teamLabel = isPlayerTurn ? "Player" : "Enemies" ;
            textCounter.text = $"TURN {TurnSystem.Instance.GetCurrentTurn()} - ({teamLabel})";
        }

        private void UpdateEndTurnButtonVisibility(bool isPlayerTurn) {
            endTurnButton.gameObject.SetActive(isPlayerTurn);
        }
    }
}