using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Windows
{
    internal static class WindowUtil
    {
        private static readonly string _projectMeta = "5325f2ad02f14e242b86eb4bb0fcb5ee";

        private static string _version;

        private static Texture2D _flexalonIcon;
        private static Texture2D _proximaIcon;
        private static Texture2D _copilotIcon;

        public static void CenterOnEditor(EditorWindow window)
        {
#if UNITY_2020_1_OR_NEWER
            var main = EditorGUIUtility.GetMainWindowPosition();
            var pos = window.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;
#endif
        }

        public static string GetVersion()
        {
            if (WindowUtil._version == null)
            {
                var version = AssetDatabase.GUIDToAssetPath(WindowUtil._projectMeta);
                var lines = File.ReadAllText(version);
                var rx = new Regex("\"version\": \"(.*?)\"");
                WindowUtil._version = rx.Match(lines).Groups[1].Value;
            }

            return WindowUtil._version;
        }

        public static bool DrawProximaButton(float width, GUIStyle style)
        {
            if (!WindowUtil._proximaIcon)
            {
                var proximaIconPath = AssetDatabase.GUIDToAssetPath("34efc6ae99ff42f438800428a52c50b5");
                WindowUtil._proximaIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(proximaIconPath);
            }

            return GUILayout.Button(WindowUtil._proximaIcon, style, GUILayout.Width(width), GUILayout.Height(width * 0.337f));
        }

        public static bool DrawCopilotButton(float width, GUIStyle style)
        {
            if (!WindowUtil._copilotIcon)
            {
                var iconPath = AssetDatabase.GUIDToAssetPath("96aaefe6c810ba6469d7e7ce04421e94");
                WindowUtil._copilotIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            }

            return GUILayout.Button(WindowUtil._copilotIcon, style, GUILayout.Width(width), GUILayout.Height(width * 0.4023f));
        }

        public static List<string> GetInstalledLayouts()
        {
            var layouts = new List<string>();
            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonAlignLayout") != null)
            {
                layouts.Add("align");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonCircleLayout") != null)
            {
                layouts.Add("circle");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonConstraint") != null)
            {
                layouts.Add("constraint");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonCurveLayout") != null)
            {
                layouts.Add("curve");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonFlexibleLayout") != null)
            {
                layouts.Add("flexible");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonGridLayout") != null)
            {
                layouts.Add("grid");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonRandomLayout") != null)
            {
                layouts.Add("random");
            }

            if (Assembly.GetAssembly(typeof(Runtime.Core.Flexalon)).GetType("Flexalon.FlexalonShapeLayout") != null)
            {
                layouts.Add("shape");
            }

            return layouts;
        }

        public static bool AllLayoutsInstalled()
        {
            return WindowUtil.GetInstalledLayouts().Count == 8;
        }
    }
}