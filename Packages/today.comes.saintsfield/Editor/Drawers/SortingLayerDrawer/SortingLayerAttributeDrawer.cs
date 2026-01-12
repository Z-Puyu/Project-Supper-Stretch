using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.SortingLayerDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.AttributePriority)]
#endif
    [CustomPropertyDrawer(typeof(SortingLayerAttribute), true)]
    public partial class SortingLayerAttributeDrawer : SaintsPropertyDrawer
    {
    }
}
