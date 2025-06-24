using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Flexalon.Editor.Windows
{
    internal class FlexalonSurvey : EditorWindow
    {
        private struct SurveyData
        {
            public string version;
            public string unityVersion;
            public string buildTarget;
            public int xr;
            public int experience;
            public string benefits;
            public string improvements;
            public string layouts;
        }

        private enum SurveyState
        {
            Ask,
            DontAsk,
            Completed
        }

        private enum XRType
        {
            None,
            XRI,
            Oculus
        }

        private static readonly string[] _options = new string[] { "Very Disappointed", "Somewhat Disappointed", "Not Disappointed" };
        private static readonly Vector2 _initialSize = new Vector2(580, 400);
        private static readonly Vector2 _expandedSize = new Vector2(580, 520);
        private static readonly string _stateKey = "FlexalonSurveyState";
        private static readonly string _dateKey = "FlexalonSurveyDate";
        private static readonly string _attemptKey = "FlexalonSurveyAttempt";
        private static readonly TimeSpan _askFrequency = new TimeSpan(3, 0, 0, 0);

        private GUIStyle _bodyStyle;
        private GUIStyle _boldStyle;
        private GUIStyle _toggleStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _dontAskButtonStyle;
        private GUIStyle _textAreaStyle;
        private SurveyData _surveyData;
        private Texture _surveyImg;

        public static bool Completed => EditorPrefs.GetInt(FlexalonSurvey._stateKey, 0) == (int)SurveyState.Completed;

        public static void ResetState()
        {
            EditorPrefs.SetInt(FlexalonSurvey._stateKey, 0);
        }

        public static bool ShouldAsk()
        {
            if (SessionState.GetBool(FlexalonSurvey._attemptKey, false))
            {
                return false;
            }

            SessionState.SetBool(FlexalonSurvey._attemptKey, true);

            if (EditorPrefs.GetInt(FlexalonSurvey._stateKey, 0) != (int)SurveyState.Ask)
            {
                return false;
            }

            if (!EditorPrefs.HasKey(FlexalonSurvey._dateKey))
            {
                EditorPrefs.SetString(FlexalonSurvey._dateKey, DateTime.Now.ToBinary().ToString());
                return false;
            }

            var lastAsked = DateTime.FromBinary(Convert.ToInt64(EditorPrefs.GetString(FlexalonSurvey._dateKey, "0")));
            if (DateTime.Now - lastAsked < FlexalonSurvey._askFrequency)
            {
                return false;
            }

            return true;
        }

        public static void ShowSurvey()
        {
            var window = EditorWindow.GetWindow<FlexalonSurvey>(true, "Flexalon Feedback", true);
            window.Show();
        }

        private void Init()
        {
            if (this._surveyData.version != null) return;

            this._bodyStyle = new GUIStyle(EditorStyles.label);
            this._bodyStyle.wordWrap = true;
            this._bodyStyle.fontSize = 14;
            this._bodyStyle.margin.left = 10;
            this._bodyStyle.margin.bottom = 10;
            this._bodyStyle.stretchWidth = false;
            this._bodyStyle.alignment = TextAnchor.MiddleCenter;

            this._boldStyle = new GUIStyle(this._bodyStyle);
            this._boldStyle.fontStyle = FontStyle.Bold;

            this._toggleStyle = new GUIStyle(EditorStyles.miniButton);
            this._toggleStyle.margin = new RectOffset(10, 10, 10, 10);
            this._toggleStyle.fixedHeight = 45;
            this._toggleStyle.fixedWidth = 180;
            this._toggleStyle.fontSize = 14;
            this._toggleStyle.alignment = TextAnchor.MiddleCenter;

            this._buttonStyle = new GUIStyle(EditorStyles.miniButton);
            this._buttonStyle.margin = new RectOffset(10, 10, 10, 10);
            this._buttonStyle.fixedHeight = 35;
            this._buttonStyle.fixedWidth = 170;
            this._buttonStyle.fontSize = 14;
            this._buttonStyle.alignment = TextAnchor.MiddleCenter;

            this._dontAskButtonStyle = new GUIStyle(EditorStyles.miniButton);
            this._dontAskButtonStyle.normal.background = null;
            this._dontAskButtonStyle.margin = new RectOffset(10, 10, 10, 10);
            this._dontAskButtonStyle.fixedWidth = 110;

            this._textAreaStyle = new GUIStyle(EditorStyles.textArea);
            this._textAreaStyle.margin.left = 10;
            this._textAreaStyle.margin.right = 10;

            this.titleContent = new GUIContent("Flexalon Feedback");

            this.minSize = this.maxSize = FlexalonSurvey._expandedSize;
            WindowUtil.CenterOnEditor(this);

            this._surveyData = new SurveyData
            {
                version = WindowUtil.GetVersion(),
                unityVersion = Application.unityVersion,
                buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString(),
#if FLEXALON_OCULUS
                xr = (int)XRType.Oculus,
#elif UNITY_XRI
                xr = (int)XRType.XRI,
#else
                xr = (int)XRType.None,
#endif
                experience = -1,
                benefits = "",
                improvements = "",
                layouts = string.Join(",", WindowUtil.GetInstalledLayouts())
            };

            var surveyImgPath = AssetDatabase.GUIDToAssetPath("0ea942e8eabc7e34c8cfd062416108ac");
            this._surveyImg = AssetDatabase.LoadAssetAtPath<Texture>(surveyImgPath);

            EditorPrefs.SetString(FlexalonSurvey._dateKey, DateTime.Now.ToBinary().ToString());
        }

        private int ToggleGroup(int selected, string[] options)
        {
            int newSelected = selected;
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < options.Length; i++)
            {
                var option = options[i];
                if (GUILayout.Toggle(selected == i, option, this._toggleStyle))
                {
                    newSelected = i;
                }

                if (i < options.Length - 1)
                {
                    GUILayout.FlexibleSpace();
                }
            }

            EditorGUILayout.EndHorizontal();
            return newSelected;
        }

        private void BeginCenter()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void EndCenter()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CenterLabel(string text, GUIStyle style)
        {
            this.BeginCenter();
            GUILayout.Label(text, style);
            this.EndCenter();
        }

        private void CenterImage(Texture image, params GUILayoutOption[] options)
        {
            this.BeginCenter();
            GUILayout.Label(image, options);
            this.EndCenter();
        }

        private void OnGUI()
        {
            this.Init();

            EditorGUILayout.BeginVertical();

                GUILayout.FlexibleSpace();

                this.BeginCenter();
                FlexalonGUI.Image("d0d1cda04ee3f144abf998efbfdfb8dc", 128, (int)(128 * 0.361f));
                this.EndCenter();

                GUILayout.FlexibleSpace();

                this.CenterLabel("Please help improve Flexalon by answering 3 quick questions.", this._boldStyle);

                if (this._surveyData.experience == -1)
                {
                    this.CenterImage(this._surveyImg, GUILayout.Width(300), GUILayout.Height(200));
                }

                this.CenterLabel("How would you feel if you could no longer use Flexalon 3D Layouts?", this._bodyStyle);

                this._surveyData.experience = this.ToggleGroup(this._surveyData.experience, FlexalonSurvey._options);

                if (this._surveyData.experience == -1)
                {
                    this.minSize = this.maxSize = FlexalonSurvey._initialSize;

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Ask me later", this._dontAskButtonStyle))
                        {
                            this.Close();
                        }
                        if (GUILayout.Button("Don't ask again", this._dontAskButtonStyle))
                        {
                            EditorPrefs.SetInt(FlexalonSurvey._stateKey, (int)SurveyState.DontAsk);
                            this.Close();
                        }
                    EditorGUILayout.EndHorizontal();
                }
                else if (this._surveyData.experience == 0 || this._surveyData.experience == 1)
                {
                    this.minSize = this.maxSize = FlexalonSurvey._expandedSize;

                    GUILayout.FlexibleSpace();

                    GUILayout.Label("What is the main benefit you get from Flexalon?", this._bodyStyle);
                    this._surveyData.benefits = GUILayout.TextArea(this._surveyData.benefits, this._textAreaStyle, GUILayout.Height(100));

                    GUILayout.FlexibleSpace();

                    GUILayout.Label("How can Flexalon be improved for you?", this._bodyStyle);
                    this._surveyData.improvements = GUILayout.TextArea(this._surveyData.improvements, this._textAreaStyle, GUILayout.Height(100));

                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Send Feedback", this._buttonStyle))
                        {
                            this.SendSurvey();
                            this.Close();
                        }
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    this.SendSurvey();
                    this.Close();
                }

            EditorGUILayout.EndVertical();
        }

        private void SendSurvey()
        {
#if UNITY_WEB_REQUEST
            var request = new UnityWebRequest("https://www.flexalon.com/api/survey", UnityWebRequest.kHttpVerbPOST);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            var json = JsonUtility.ToJson(this._surveyData);
            var jsonData = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonData);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SendWebRequest().completed += op => {
                if (request.responseCode == 200)
                {
                    Debug.Log("Flexalon feedback sent successfully.");
                    EditorPrefs.SetInt(FlexalonSurvey._stateKey, (int)SurveyState.Completed);
                }
                else if (request.responseCode == 400)
                {
                    Debug.LogError("Failed to send Flexalon feedback: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("Failed to send Flexalon feedback: " + request.error);

                }

                request.Dispose();
            };
#else
            EditorPrefs.SetInt(_stateKey, (int)SurveyState.Completed);
#endif
        }
    }
}