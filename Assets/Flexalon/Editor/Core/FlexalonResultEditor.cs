using Flexalon.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Core
{
    [CustomEditor(typeof(FlexalonResult))]
    public class FlexalonResultEditor : UnityEditor.Editor
    {
        void OnEnable()
        {
            foreach (var target in this.targets)
            {
                target.hideFlags = HideFlags.HideInInspector;
            }
        }
    }
}