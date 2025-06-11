using SaintsField.Editor.Core;
using UnityEditor;

namespace SaintsField.Editor.Drawers.OverlayRichLabelDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.SuperPriority)]
#endif
    [CustomPropertyDrawer(typeof(OverlayRichLabelAttribute), true)]
    public partial class OverlayRichLabelAttributeDrawer: SaintsPropertyDrawer
    {

#if UNITY_2021_3_OR_NEWER

#endif
    }
}
