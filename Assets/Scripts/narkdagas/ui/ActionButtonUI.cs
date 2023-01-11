using narkdagas.tbcs;
using narkdagas.tbcs.actions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace narkdagas.ui {
    public class ActionButtonUI : MonoBehaviour {
        [SerializeField] private Button button;
        [FormerlySerializedAs("text")] [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Image selectedImage;
        private BaseAction _baseAction;
        
        public void SetBaseAction(BaseAction baseAction) {
            _baseAction = baseAction;
            buttonText.text = baseAction.GetActionNameLabel().ToUpper();
            button.onClick.AddListener(() => {
                UnitActionSystem.Instance.SetSelectedAction(baseAction);
            });
        }

        public bool UpdateSelectedVisual() {
            var baseAction = UnitActionSystem.Instance.GetSelectedAction();
            selectedImage.enabled = _baseAction == baseAction;
            return selectedImage.enabled;
        }
    }
}