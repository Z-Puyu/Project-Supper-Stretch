using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Windows
{
    [InitializeOnLoad]
    internal class FlexalonMenu : EditorWindow
    {
        private static readonly string _website = "https://www.flexalon.com?utm_source=fxmenu";
        public static readonly string StoreLink = "https://assetstore.unity.com/packages/tools/utilities/flexalon-3d-layouts-230509?aid=1101lqSYn";
        private static readonly string _review = "https://assetstore.unity.com/packages/tools/utilities/flexalon-3d-layouts-230509#reviews";
        private static readonly string _discord = "https://discord.gg/VM9cWJ9rjH";
        private static readonly string _docs = "https://www.flexalon.com/docs?utm_source=fxmenu";
        private static readonly string _templates = "https://www.flexalon.com/templates?utm_source=fxmenu";
        private static readonly string _examples = "https://github.com/afarchy/flexalon-examples";
        // private static readonly string _proxima = "https://www.unityproxima.com?utm_source=pxmenu";
        // private static readonly string _copilot = "https://www.flexalon.com/ai?utm_source=pxmenu";
        private static readonly string _buildalon = "https://www.buildalon.com?utm_source=fxmenu";

        private static readonly string _showOnStartKey = "FlexalonMenu_ShowOnStart";
        private static readonly string _versionKey = "FlexalonMenu_Version";

        private GUIStyle _errorStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _versionStyle;
        private GUIStyle _boldStyle;
        private GUIStyle _semiboldStyle;
        private GUIStyle _moreToolsButtonStyle;
        private GUIStyle _moreLayoutsStyle;
        private GUIStyle _buildalonStyle;

        private static ShowOnStart _showOnStart;
        private static readonly string[] _showOnStartOptions = {
            "Always", "On Update", "Never"
        };

        private Vector2 _scrollPosition;

        private List<string> _changelog = new List<string>();

        private bool _haveAllLayouts = false;

        private enum ShowOnStart
        {
            Always,
            OnUpdate,
            Never
        }

        static FlexalonMenu()
        {
            EditorApplication.update += FlexalonMenu.OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            EditorApplication.update -= FlexalonMenu.OnEditorUpdate;
            FlexalonMenu.Initialize();
        }

        internal static void Initialize()
        {
            var shownKey = "FlexalonMenuShown";
            bool alreadyShown = SessionState.GetBool(shownKey, false);
            SessionState.SetBool(shownKey, true);

            var version = WindowUtil.GetVersion();
            var lastVersion = EditorPrefs.GetString(FlexalonMenu._versionKey, "0.0.0");
            var newVersion = version.CompareTo(lastVersion) > 0;
            if (newVersion)
            {
                EditorPrefs.SetString(FlexalonMenu._versionKey, version);
                alreadyShown = false;
            }

            FlexalonMenu._showOnStart = (ShowOnStart)EditorPrefs.GetInt(FlexalonMenu._showOnStartKey, 0);
            bool showPref = FlexalonMenu._showOnStart == ShowOnStart.Always ||
                (FlexalonMenu._showOnStart == ShowOnStart.OnUpdate && newVersion);
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !alreadyShown && showPref && !Application.isBatchMode)
            {
                FlexalonMenu.StartScreen();
            }

            if (!EditorApplication.isPlayingOrWillChangePlaymode && FlexalonSurvey.ShouldAsk())
            {
                FlexalonSurvey.ShowSurvey();
            }
        }

        private void OnDisable()
        {
            this._bodyStyle = null;
            FlexalonGUI.CleanupBackgroundTextures(FlexalonMenu.StyleTag);
        }

        [MenuItem("Tools/Flexalon/Start Screen")]
        public static void StartScreen()
        {
            FlexalonMenu window = EditorWindow.GetWindow<FlexalonMenu>(true, "Flexalon Start Screen", true);
            window.minSize = new Vector2(800, 600);
            window.maxSize = window.minSize;
            window.Show();
        }

        [MenuItem("Tools/Flexalon/Website")]
        public static void OpenStore()
        {
            Application.OpenURL(FlexalonMenu._website);
        }

        [MenuItem("Tools/Flexalon/Write a Review")]
        public static void OpenReview()
        {
            Application.OpenURL(FlexalonMenu._review);
        }

        [MenuItem("Tools/Flexalon/Support (Discord)")]
        public static void OpenSupport()
        {
            Application.OpenURL(FlexalonMenu._discord);
        }

        private const string StyleTag = "FlexalonStartScreenStyles";

        private void InitStyles()
        {
            if (this._bodyStyle != null) return;

            FlexalonGUI.StyleTag = FlexalonMenu.StyleTag;
            FlexalonGUI.StyleFontSize = 14;

            this._bodyStyle = new GUIStyle(EditorStyles.label);
            this._bodyStyle.wordWrap = true;
            this._bodyStyle.fontSize = 14;
            this._bodyStyle.margin.left = 10;
            this._bodyStyle.margin.top = 10;
            this._bodyStyle.stretchWidth = false;
            this._bodyStyle.richText = true;

            this._buildalonStyle = FlexalonGUI.CreateStyle(FlexalonGUI.HexColor("#FF1E6F"));
            this._buildalonStyle.fontStyle = FontStyle.Bold;
            this._buildalonStyle.margin.left = 10;
            this._buildalonStyle.margin.top = 10;

            this._boldStyle = new GUIStyle(this._bodyStyle);
            this._boldStyle.fontStyle = FontStyle.Bold;
            this._boldStyle.fontSize = 16;

            this._semiboldStyle = new GUIStyle(this._bodyStyle);
            this._semiboldStyle.fontStyle = FontStyle.Bold;

            this._errorStyle = new GUIStyle(this._bodyStyle);
            this._errorStyle.fontStyle = FontStyle.Bold;
            this._errorStyle.margin.top = 10;
            this._errorStyle.normal.textColor = new Color(1, 0.2f, 0);

            this._buttonStyle = new GUIStyle(this._bodyStyle);
            this._buttonStyle.fontSize = 14;
            this._buttonStyle.margin.bottom = 5;
            this._buttonStyle.padding.top = 5;
            this._buttonStyle.padding.left = 10;
            this._buttonStyle.padding.right = 10;
            this._buttonStyle.padding.bottom = 5;
            this._buttonStyle.hover.background = Texture2D.grayTexture;
            this._buttonStyle.hover.textColor = Color.white;
            this._buttonStyle.active.background = Texture2D.grayTexture;
            this._buttonStyle.active.textColor = Color.white;
            this._buttonStyle.focused.background = Texture2D.grayTexture;
            this._buttonStyle.focused.textColor = Color.white;
            this._buttonStyle.normal.background = Texture2D.grayTexture;
            this._buttonStyle.normal.textColor = Color.white;
            this._buttonStyle.wordWrap = false;
            this._buttonStyle.stretchWidth = false;

            this._versionStyle = new GUIStyle(EditorStyles.label);
            this._versionStyle.padding.right = 10;

            this._moreToolsButtonStyle = new GUIStyle(this._buttonStyle);
            this._moreToolsButtonStyle.normal.background = Texture2D.blackTexture;
            this._moreToolsButtonStyle.hover.background = Texture2D.blackTexture;
            this._moreToolsButtonStyle.focused.background = Texture2D.blackTexture;
            this._moreToolsButtonStyle.active.background = Texture2D.blackTexture;
            this._moreToolsButtonStyle.padding.left = 0;
            this._moreToolsButtonStyle.padding.right = 0;
            this._moreToolsButtonStyle.padding.bottom = 0;
            this._moreToolsButtonStyle.padding.top = 0;
            this._moreToolsButtonStyle.margin.bottom = 20;

            this._moreLayoutsStyle = new GUIStyle(this._buttonStyle);
            this._moreLayoutsStyle.normal.background = new Texture2D(1, 1);
            this._moreLayoutsStyle.normal.background.SetPixel(0, 0, new Color(0.18f, 0.47f, 0.63f));
            this._moreLayoutsStyle.normal.background.Apply();
            this._moreLayoutsStyle.hover.background = this._moreLayoutsStyle.normal.background;
            this._moreLayoutsStyle.focused.background = this._moreLayoutsStyle.normal.background;
            this._moreLayoutsStyle.active.background = this._moreLayoutsStyle.normal.background;
            this._moreLayoutsStyle.normal.textColor = Color.white;
            this._moreLayoutsStyle.fontStyle = FontStyle.Bold;

            WindowUtil.CenterOnEditor(this);

            this.ReadChangeLog();

            this._haveAllLayouts = WindowUtil.AllLayoutsInstalled();
        }

        private void LinkButton(string label, string url, GUIStyle style = null, int width = 170)
        {
            if (style == null) style = this._buttonStyle;
            var labelContent = new GUIContent(label);
            var position = GUILayoutUtility.GetRect(width, 35, style);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            if (GUI.Button(position, labelContent, style))
            {
                Application.OpenURL(url);
            }
        }

        private bool Button(string label, GUIStyle style = null, int width = 170)
        {
            if (style == null) style = this._buttonStyle;
            var labelContent = new GUIContent(label);
            var position = GUILayoutUtility.GetRect(width, 35, style);
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, labelContent, style);
        }

        private void Bullet(string text)
        {
            var ws = 1 + text.IndexOf('-');
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < ws; i++)
            {
                GUILayout.Space(10);
            }
            GUILayout.Label("•", this._bodyStyle);

            GUILayout.Label(text.Substring(ws + 1), this._bodyStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();
        }

        private void ReadChangeLog()
        {
            this._changelog.Clear();
            var changelogPath = AssetDatabase.GUIDToAssetPath("b711ce346029a6f43969ef8de5691942");
            var changelogAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(changelogPath);
            this._changelog = changelogAsset.text.Split('\n')
                                            .Select(x => Regex.Replace(x.TrimEnd(), @"`(.*?)`", "<b>$1</b>"))
                                            .Select(x => Regex.Replace(x.TrimEnd(), @"\*\*(.*?)\*\*", "<b>$1</b>"))
                                            .Where(x => !string.IsNullOrEmpty(x))
                                            .ToList();
            var start = this._changelog.FindIndex(l => l.StartsWith("## "));
            var end = this._changelog.FindIndex(start + 1, l => l.StartsWith("---"));
            this._changelog = this._changelog.GetRange(start, end - start);
        }

        private void WhatsNew()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("What's New in Flexalon", this._boldStyle);
            EditorGUILayout.Space();

            for (int i = 0; i < this._changelog.Count; i++)
            {
                var line = this._changelog[i];
                if (line.StartsWith("###"))
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(line.Substring(4), this._semiboldStyle);
                    EditorGUILayout.Space();
                }
                else if (line.StartsWith("##"))
                {
                    EditorGUILayout.Space();
                    GUILayout.Label(line.Substring(3), this._boldStyle, GUILayout.ExpandWidth(true));
                    EditorGUILayout.Space();
                }
                else
                {
                    this.Bullet(line);
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.Space();
        }

        private void OnGUI()
        {
            this.InitStyles();

            GUILayout.BeginHorizontal("In BigTitle", GUILayout.ExpandWidth(true))   ;
            {
                FlexalonGUI.Image("d0d1cda04ee3f144abf998efbfdfb8dc", 128, (int)(128 * 0.361f));
                GUILayout.FlexibleSpace();
                GUILayout.Label("Version: " + WindowUtil.GetVersion(), this._versionStyle, GUILayout.ExpandHeight(true));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label("Resources", this._boldStyle);
                    this.LinkButton("Discord Invite", FlexalonMenu._discord);
                    this.LinkButton("Documentation", FlexalonMenu._docs);
                    if (this._haveAllLayouts)
                    {
                        this.LinkButton("Templates", FlexalonMenu._templates);
                        this.LinkButton("More Examples", FlexalonMenu._examples);
                    }
                    else
                    {
                        this.LinkButton("Get More Layouts", FlexalonMenu._website, this._moreLayoutsStyle);
                    }

                    this.LinkButton("Write a Review", FlexalonMenu._review);

                    if (!FlexalonSurvey.Completed)
                    {
                        if (this.Button("Feedback"))
                        {
                            FlexalonSurvey.ShowSurvey();
                        }
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.Label("More Tools", this._boldStyle);
                    if (FlexalonGUI.ImageButton("2d4f1ef6bb116dd439a01757e51b59de", 165, (int)(165 * 0.525f)))
                    {
                        Application.OpenURL(FlexalonMenu._buildalon);
                    }

                    EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                }
                GUILayout.EndVertical();

                EditorGUILayout.Separator();

                GUILayout.BeginVertical();
                {
                    this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition);

                    GUILayout.Label("Thank you for using Flexalon!", this._boldStyle);

                    EditorGUILayout.Space();

                    GUILayout.Label("You're invited to join the Discord community for support and feedback. Let us know how to make Flexalon better for you!", this._bodyStyle);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    FlexalonGUI.Vertical(EditorStyles.helpBox, () =>
                    {
                        GUILayout.Label("Unveiling our new tool for Unity developers:", this._bodyStyle);
                        EditorGUILayout.Space();
                        if (FlexalonGUI.Link("Buildalon: Automate Unity!", this._buildalonStyle))
                        {
                            Application.OpenURL(FlexalonMenu._buildalon);
                        }
                        EditorGUILayout.Space();
                        GUILayout.Label("Buildalon is a comprehensive suite of build, test, and deploy automation solutions for Unity developers.", this._bodyStyle);
                        EditorGUILayout.Space();
                    });

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    FlexalonGUI.Vertical(EditorStyles.helpBox, () =>
                    {
                        GUILayout.Label("If you're enjoying Flexalon, please consider writing a review. It helps a ton!", this._bodyStyle);
                        EditorGUILayout.Space();
                    });

                    this.WhatsNew();

                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("In BigTitle", GUILayout.ExpandHeight(true));
            {
                GUILayout.Label("Tools/Flexalon/Start Screen");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Show On Start: ");
                var newShowOnStart = (ShowOnStart)EditorGUILayout.Popup((int)FlexalonMenu._showOnStart, FlexalonMenu._showOnStartOptions);
                if (FlexalonMenu._showOnStart != newShowOnStart)
                {
                    FlexalonMenu._showOnStart = newShowOnStart;
                    EditorPrefs.SetInt(FlexalonMenu._showOnStartKey, (int)FlexalonMenu._showOnStart);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}