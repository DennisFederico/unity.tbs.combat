using System;
using System.Collections.Generic;
using narkdagas.tbcs;
using narkdagas.tbcs.actions;
using TMPro;
using UnityEngine;

namespace narkdagas.ui {
    public class UnitActionSystemUI : MonoBehaviour {
        public static UnitActionSystemUI Instance { get; private set; }

        [SerializeField] private RectTransform actionButtonPrefab;
        [SerializeField] private RectTransform actionButtonContainer;
        [SerializeField] private TextMeshProUGUI apText;
        private List<ActionButtonUI> _actionButtonList;

        private void Awake() {
            if (Instance != null) {
                Debug.LogError($"There's more than one UnitActionSystemUIS in the scene! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _actionButtonList = new List<ActionButtonUI>();
        }

        private void Start() {
            UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            UnitActionSystem.Instance.OnActionRunningChanged += UnitActionSystem_OnActionRunningChanged;
            TurnSystem.Instance.OnTurnChanged += (_, _) => UpdateActionPoints();
            CreateUnitActionButtons();
        }

        private void CreateUnitActionButtons() {
            foreach (RectTransform actionButtonRectTransform in actionButtonContainer) {
                Destroy(actionButtonRectTransform.gameObject);
            }
            _actionButtonList.Clear();
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit) {
                foreach (BaseAction action in selectedUnit.GetActions()) {
                    RectTransform actionButtonRect = Instantiate(actionButtonPrefab, actionButtonContainer);
                    var actionButtonUI = actionButtonRect.GetComponent<ActionButtonUI>();
                    actionButtonUI.SetBaseAction(action);
                    _actionButtonList.Add(actionButtonUI);
                }
            }

            UpdateActionPoints();
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs args) {
            CreateUnitActionButtons();
            UpdateSelectedActionVisual();
            UpdateActionPoints();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs args) {
            UpdateSelectedActionVisual();
        }
        
        private void UnitActionSystem_OnActionRunningChanged(object sender, bool actionRunning) {
            SetShowActionsVisual(!actionRunning);
            UpdateActionPoints();
        }

        private void UpdateActionPoints() {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit) {
                apText.text = $"Remaining APs: {selectedUnit.GetActionPoints()}";
                SetShowAPText(true);
            } else {
                SetShowAPText(false);
            }
        }
        
        public void UpdateSelectedActionVisual() {
            foreach (var actionButtonUI in _actionButtonList) {
                actionButtonUI.UpdateSelectedVisual();
            }
        }

        public void SetShowActionsVisual(bool show) {
            actionButtonContainer.gameObject.SetActive(show);
        }

        public void SetShowAPText(bool show) {
            apText.gameObject.SetActive(show);
        }
    }
}