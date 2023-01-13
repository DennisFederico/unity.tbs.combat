using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace narkdagas.tbcs.unit {
    public class UnitWorldUI : MonoBehaviour {
        [SerializeField] private Unit unit;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image healthBar;
        [SerializeField] private HealthSystem healthSystem;
        private EventHandler _onActionPointsChangedAction;
        private EventHandler _onHealthChangedAction;

        private void Start() {
            _onActionPointsChangedAction = (_,_) => { UpdateActionPointsText(); };
            unit.OnActionPointsChanged += _onActionPointsChangedAction;
            _onHealthChangedAction = (_, _) => { UpdateHealthBar(); };
            healthSystem.OnHealthChanged += _onHealthChangedAction;
            UpdateActionPointsText();
            UpdateHealthBar();
        }

        private void UpdateActionPointsText() {
            text.text = unit.GetActionPoints().ToString();
        }

        private void UpdateHealthBar() {
            healthBar.fillAmount = healthSystem.GetNormalizedHealth();
        }

        private void OnDestroy() {
            unit.OnActionPointsChanged -= _onActionPointsChangedAction;
            healthSystem.OnHealthChanged -= _onHealthChangedAction;
        }
    }
}