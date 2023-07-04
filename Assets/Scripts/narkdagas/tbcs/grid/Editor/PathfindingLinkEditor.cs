using UnityEditor;
using UnityEngine;

namespace narkdagas.tbcs.grid.Editor {
    
    [CustomEditor(typeof(PathfindingLinkMono))]
    public class PathfindingLinkEditor : UnityEditor.Editor {
        private void OnSceneGUI() {
            var pathfindingLinkMono = (PathfindingLinkMono) target;
            EditorGUI.BeginChangeCheck();
            var positionA = Handles.PositionHandle(pathfindingLinkMono.positionA, Quaternion.identity);
            var positionB = Handles.PositionHandle(pathfindingLinkMono.positionB, Quaternion.identity);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(pathfindingLinkMono, "Change Pathfinding Link position");
                pathfindingLinkMono.positionA = positionA;
                pathfindingLinkMono.positionB = positionB;
            }
        }
    }
}