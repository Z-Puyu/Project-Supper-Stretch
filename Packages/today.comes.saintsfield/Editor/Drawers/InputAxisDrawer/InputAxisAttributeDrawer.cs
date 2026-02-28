#if UNITY_2021_3_OR_NEWER
#endif
using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.InputAxisDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.AttributePriority)]
#endif
    [CustomPropertyDrawer(typeof(InputAxisAttribute), true)]
    public partial class InputAxisAttributeDrawer: SaintsPropertyDrawer
    {

    }
}
