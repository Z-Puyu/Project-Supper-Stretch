#if UNITY_2021_3_OR_NEWER
#endif
using System.Collections.Generic;
using SaintsField.Editor.Core;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SaintsField.Editor.Drawers.InputAxisDrawer
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.Editor.DrawerPriority(Sirenix.OdinInspector.Editor.DrawerPriorityLevel.AttributePriority)]
#endif
    [CustomPropertyDrawer(typeof(InputAxisAttribute), true)]
    public partial class InputAxisAttributeDrawer: SaintsPropertyDrawer
    {
        private static IReadOnlyList<string> GetAxisNames()
        {
            SerializedObject inputAssetSettings = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/InputManager.asset"));
            SerializedProperty axesProperty = inputAssetSettings.FindProperty("m_Axes");
            List<string> axisNames = new List<string>();
            for (int index = 0; index < axesProperty.arraySize; index++)
            {
                axisNames.Add(axesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("m_Name").stringValue);
            }

            return axisNames;
        }

        private static void OpenInputManager()
        {
            SettingsService.OpenProjectSettings("Project/Input Manager");
        }
    }
}
