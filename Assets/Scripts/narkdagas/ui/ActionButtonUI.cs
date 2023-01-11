using narkdagas.tbcs;
using narkdagas.tbcs.actions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace narkdagas.ui {
    public class ActionButtonUI : MonoBehaviour {
        [FormerlySerializedAs("text")] [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button button;

        public void SetBaseAction(BaseAction baseAction) {
            buttonText.text = baseAction.GetActionNameLabel().ToUpper();
            button.onClick.AddListener(() => {
                UnitActionSystem.Instance.SetSelectedAction(baseAction);
            });
        }
    }
}