using TMPro;
using UnityEngine;

namespace narkdagas.tbcs.grid {
    public class GridPathfindingDebug : GridDebugObject {
        
        [SerializeField] private TextMeshPro gCostText;
        [SerializeField] private TextMeshPro hCostText;
        [SerializeField] private TextMeshPro fCostText;
        [SerializeField] private SpriteRenderer isWalkable;
        private Color _greenColor = new Color(0, 1, 0, .5f);
        private Color _redColor = new Color(1, 0, 0, .5f);
        private PathNode pathNode;

        public override void SetGridObject(object gridObject) {
            pathNode = (PathNode)gridObject;
            base.SetGridObject(gridObject);
        }

        protected override void Update() {
            base.Update();
            gCostText.text = NumDigits(pathNode.GCost) > 3 ? "Inf" : pathNode.GCost.ToString();
            hCostText.text = NumDigits(pathNode.HCost) > 3 ? "Inf" : pathNode.HCost.ToString();
            fCostText.text = NumDigits(pathNode.FCost) > 3 ? "Inf" : pathNode.FCost.ToString();
            isWalkable.color = pathNode.IsWalkable ? _greenColor : _redColor;
        }
        
        private int NumDigits (long n) =>  n == 0L ? 1 : (n > 0L ? 1 : 2) + (int)Mathf.Log10(Mathf.Abs(n));
    }
}