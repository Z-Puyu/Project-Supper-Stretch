using Flexalon.Editor.Core;
using Flexalon.Runtime.Interaction;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Interaction
{
    [CustomEditor(typeof(FlexalonDragTarget)), CanEditMultipleObjects]
    public class FlexalonDragTargetEditor : FlexalonComponentEditor
    {
        void OnSceneGUI()
        {
            // Draw a box at the transforms position
            var script = this.target as FlexalonDragTarget;
            var node = Runtime.Core.Flexalon.GetNode(script.gameObject);
            if (node == null || node.Result == null)
            {
                return;
            }

            var r = node.Result;

            // Draw hit box for drag target if margin is not zero.
            Handles.color = Color.green;
            var worldBoxScale = node.GetWorldBoxScale(true);
            Handles.matrix = Matrix4x4.TRS(node.GetWorldBoxPosition(worldBoxScale, false), script.transform.rotation, worldBoxScale);
            Handles.DrawWireCube(Vector3.zero, r.AdapterBounds.size + script.Margin * 2);
        }
    }
}