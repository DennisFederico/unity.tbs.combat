using System;
using narkdagas.tbcs;
using narkdagas.tbcs.actions;
using UnityEngine;

namespace narkdagas.ui {
    public class UnitActionSystemUI : MonoBehaviour {
        [SerializeField] private RectTransform actionButtonPrefab;
        [SerializeField] private RectTransform actionButtonContainer;

        private void Start() {
            CreateUnitActionButtons();
            UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChanged;
        }

        private void CreateUnitActionButtons() {
            foreach (RectTransform actionButtonRectTransform in actionButtonContainer) {
                Destroy(actionButtonRectTransform.gameObject);
            }

            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit) {
                foreach (BaseAction action in selectedUnit.GetActions()) {
                    RectTransform actionButtonRect = Instantiate(actionButtonPrefab, actionButtonContainer);
                    actionButtonRect.GetComponent<ActionButtonUI>().SetBaseAction(action);
                }
            }
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs args) {
            CreateUnitActionButtons();
        }
    }
}