using Flexalon.Runtime.Core;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Core
{
    [CustomEditor(typeof(FlexalonObject)), CanEditMultipleObjects]
    public class FlexalonObjectEditor : FlexalonComponentEditor
    {
        private SerializedProperty _width;
        private SerializedProperty _widthType;
        private SerializedProperty _widthOfParent;
        private SerializedProperty _height;
        private SerializedProperty _heightType;
        private SerializedProperty _heightOfParent;
        private SerializedProperty _depth;
        private SerializedProperty _depthType;
        private SerializedProperty _depthOfParent;
        private SerializedProperty _minWidth;
        private SerializedProperty _minWidthType;
        private SerializedProperty _minWidthOfParent;
        private SerializedProperty _minHeight;
        private SerializedProperty _minHeightType;
        private SerializedProperty _minHeightOfParent;
        private SerializedProperty _minDepth;
        private SerializedProperty _minDepthType;
        private SerializedProperty _minDepthOfParent;
        private SerializedProperty _maxWidth;
        private SerializedProperty _maxWidthType;
        private SerializedProperty _maxWidthOfParent;
        private SerializedProperty _maxHeight;
        private SerializedProperty _maxHeightType;
        private SerializedProperty _maxHeightOfParent;
        private SerializedProperty _maxDepth;
        private SerializedProperty _maxDepthType;
        private SerializedProperty _maxDepthOfParent;
        private SerializedProperty _offset;
        private SerializedProperty _rotation;
        private SerializedProperty _scale;
        private SerializedProperty _marginLeft;
        private SerializedProperty _marginRight;
        private SerializedProperty _marginTop;
        private SerializedProperty _marginBottom;
        private SerializedProperty _marginFront;
        private SerializedProperty _marginBack;
        private SerializedProperty _paddingLeft;
        private SerializedProperty _paddingRight;
        private SerializedProperty _paddingTop;
        private SerializedProperty _paddingBottom;
        private SerializedProperty _paddingFront;
        private SerializedProperty _paddingBack;
        private SerializedProperty _skipLayout;
        private SerializedProperty _useDefaultAdapter;

        private static readonly int ValueWidth = 50;

        [MenuItem("GameObject/Flexalon/Flexalon Object")]
        public static void Create(MenuCommand command)
        {
            FlexalonComponentEditor.Create<FlexalonObject>("Flexalon Object", command.context);
        }

        void OnEnable()
        {
            this._width = this.serializedObject.FindProperty("_width");
            this._widthType = this.serializedObject.FindProperty("_widthType");
            this._widthOfParent = this.serializedObject.FindProperty("_widthOfParent");
            this._height = this.serializedObject.FindProperty("_height");
            this._heightType = this.serializedObject.FindProperty("_heightType");
            this._heightOfParent = this.serializedObject.FindProperty("_heightOfParent");
            this._depth = this.serializedObject.FindProperty("_depth");
            this._depthType = this.serializedObject.FindProperty("_depthType");
            this._depthOfParent = this.serializedObject.FindProperty("_depthOfParent");
            this._minWidth = this.serializedObject.FindProperty("_minWidth");
            this._minWidthType = this.serializedObject.FindProperty("_minWidthType");
            this._minWidthOfParent = this.serializedObject.FindProperty("_minWidthOfParent");
            this._minHeight = this.serializedObject.FindProperty("_minHeight");
            this._minHeightType = this.serializedObject.FindProperty("_minHeightType");
            this._minHeightOfParent = this.serializedObject.FindProperty("_minHeightOfParent");
            this._minDepth = this.serializedObject.FindProperty("_minDepth");
            this._minDepthType = this.serializedObject.FindProperty("_minDepthType");
            this._minDepthOfParent = this.serializedObject.FindProperty("_minDepthOfParent");
            this._maxWidth = this.serializedObject.FindProperty("_maxWidth");
            this._maxWidthType = this.serializedObject.FindProperty("_maxWidthType");
            this._maxWidthOfParent = this.serializedObject.FindProperty("_maxWidthOfParent");
            this._maxHeight = this.serializedObject.FindProperty("_maxHeight");
            this._maxHeightType = this.serializedObject.FindProperty("_maxHeightType");
            this._maxHeightOfParent = this.serializedObject.FindProperty("_maxHeightOfParent");
            this._maxDepth = this.serializedObject.FindProperty("_maxDepth");
            this._maxDepthType = this.serializedObject.FindProperty("_maxDepthType");
            this._maxDepthOfParent = this.serializedObject.FindProperty("_maxDepthOfParent");
            this._offset = this.serializedObject.FindProperty("_offset");
            this._rotation = this.serializedObject.FindProperty("_rotation");
            this._scale = this.serializedObject.FindProperty("_scale");
            this._marginLeft = this.serializedObject.FindProperty("_marginLeft");
            this._marginRight = this.serializedObject.FindProperty("_marginRight");
            this._marginTop = this.serializedObject.FindProperty("_marginTop");
            this._marginBottom = this.serializedObject.FindProperty("_marginBottom");
            this._marginFront = this.serializedObject.FindProperty("_marginFront");
            this._marginBack = this.serializedObject.FindProperty("_marginBack");
            this._paddingLeft = this.serializedObject.FindProperty("_paddingLeft");
            this._paddingRight = this.serializedObject.FindProperty("_paddingRight");
            this._paddingTop = this.serializedObject.FindProperty("_paddingTop");
            this._paddingBottom = this.serializedObject.FindProperty("_paddingBottom");
            this._paddingFront = this.serializedObject.FindProperty("_paddingFront");
            this._paddingBack = this.serializedObject.FindProperty("_paddingBack");
            this._skipLayout = this.serializedObject.FindProperty("_skipLayout");
            this._useDefaultAdapter = this.serializedObject.FindProperty("_useDefaultAdapter");

            FlexalonObjectEditor._sizeFoldout = EditorPrefs.GetBool("FlexalonSizeFoldout", true);
            FlexalonObjectEditor._minMaxFoldout = EditorPrefs.GetBool("FlexalonMinMaxFoldout", false);
            FlexalonObjectEditor._marginFoldout = EditorPrefs.GetBool("FlexalonMarginFoldout", false);
            FlexalonObjectEditor._paddingFoldout = EditorPrefs.GetBool("FlexalonPaddingFoldout", false);
            FlexalonObjectEditor._transformFoldout = EditorPrefs.GetBool("FlexalonTransformFoldout", false);
        }

        void OnDisable()
        {
            EditorPrefs.SetBool("FlexalonSizeFoldout", FlexalonObjectEditor._sizeFoldout);
            EditorPrefs.SetBool("FlexalonMinMaxFoldout", FlexalonObjectEditor._minMaxFoldout);
            EditorPrefs.SetBool("FlexalonMarginFoldout", FlexalonObjectEditor._marginFoldout);
            EditorPrefs.SetBool("FlexalonPaddingFoldout", FlexalonObjectEditor._paddingFoldout);
            EditorPrefs.SetBool("FlexalonTransformFoldout", FlexalonObjectEditor._transformFoldout);
        }

        private static bool _sizeFoldout;
        private static bool _minMaxFoldout;
        private static bool _marginFoldout;
        private static bool _paddingFoldout;
        private static bool _transformFoldout;

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.ForceUpdateButton();

            FlexalonObjectEditor._sizeFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(FlexalonObjectEditor._sizeFoldout, "Size");
            if (FlexalonObjectEditor._sizeFoldout)
            {
                this.CreateSizeProperty("Width", this._widthType, this._width, this._widthOfParent);
                this.CreateSizeProperty("Height", this._heightType, this._height, this._heightOfParent);
                this.CreateSizeProperty("Depth", this._depthType, this._depth, this._depthOfParent);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            FlexalonObjectEditor._minMaxFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(FlexalonObjectEditor._minMaxFoldout, "Min / Max");
            if (FlexalonObjectEditor._minMaxFoldout)
            {
                this.CreateMinMaxSizeProperty("Width", this._minWidthType, this._minWidth, this._minWidthOfParent, this._maxWidthType, this._maxWidth, this._maxWidthOfParent);
                this.CreateMinMaxSizeProperty("Height", this._minHeightType, this._minHeight, this._minHeightOfParent, this._maxHeightType, this._maxHeight, this._maxHeightOfParent);
                this.CreateMinMaxSizeProperty("Depth", this._minDepthType, this._minDepth, this._minDepthOfParent, this._maxDepthType, this._maxDepth, this._maxDepthOfParent);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            FlexalonObjectEditor._marginFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(FlexalonObjectEditor._marginFoldout, "Margin");
            if (FlexalonObjectEditor._marginFoldout)
            {
                this.CreateSpacingProperty("Left", "Right", this._marginLeft, this._marginRight);
                this.CreateSpacingProperty("Top", "Bottom", this._marginTop, this._marginBottom);
                this.CreateSpacingProperty("Front", "Back", this._marginFront, this._marginBack);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            FlexalonObjectEditor._paddingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(FlexalonObjectEditor._paddingFoldout, "Padding");
            if (FlexalonObjectEditor._paddingFoldout)
            {
                this.CreateSpacingProperty("Left", "Right", this._paddingLeft, this._paddingRight);
                this.CreateSpacingProperty("Top", "Bottom", this._paddingTop, this._paddingBottom);
                this.CreateSpacingProperty("Front", "Back", this._paddingFront, this._paddingBack);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            FlexalonObjectEditor._transformFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(FlexalonObjectEditor._transformFoldout, "Transform");
            if (FlexalonObjectEditor._transformFoldout)
            {
                EditorGUILayout.PropertyField(this._offset);
                EditorGUILayout.PropertyField(this._rotation);
                EditorGUILayout.PropertyField(this._scale);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.PropertyField(this._skipLayout);
            EditorGUILayout.PropertyField(this._useDefaultAdapter);

            this.ApplyModifiedProperties();
        }

        private void CreateSizeProperty(string label, SerializedProperty typeProperty, SerializedProperty fixedProperty, SerializedProperty ofParentProperty)
        {
            EditorGUILayout.BeginHorizontal();

            bool showTypeLabel = true;
            if (typeProperty.enumValueIndex == (int)SizeType.Fixed)
            {
                EditorGUILayout.PropertyField(fixedProperty, new GUIContent(label));
                showTypeLabel = false;
            }
            else if (typeProperty.enumValueIndex == (int)SizeType.Fill)
            {
                EditorGUILayout.PropertyField(ofParentProperty, new GUIContent(label));
                showTypeLabel = false;
            }

            EditorGUILayout.PropertyField(typeProperty, showTypeLabel ? new GUIContent(label) : GUIContent.none);

            EditorGUILayout.EndHorizontal();
        }

        private void CreateMinMaxSizeProperty(string label, SerializedProperty minTypeProperty, SerializedProperty minFixedProperty, SerializedProperty minOfParentProperty,
            SerializedProperty maxTypeProperty, SerializedProperty maxFixedProperty, SerializedProperty maxOfParentProperty)
        {
            EditorGUILayout.BeginHorizontal();

            var labelWidth = EditorGUIUtility.labelWidth;

            if (minTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fixed)
            {
                EditorGUILayout.PropertyField(minFixedProperty, new GUIContent(label), GUILayout.Width(EditorGUIUtility.labelWidth + FlexalonObjectEditor.ValueWidth + 3));
            }
            else if (minTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fill)
            {
                EditorGUILayout.PropertyField(minOfParentProperty, new GUIContent(label), GUILayout.Width(EditorGUIUtility.labelWidth + FlexalonObjectEditor.ValueWidth + 3));
            }
            else
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("-", GUILayout.Width(FlexalonObjectEditor.ValueWidth));
                EditorGUI.EndDisabledGroup();
            }

            EditorGUIUtility.labelWidth = 20;

            EditorGUILayout.PropertyField(minTypeProperty, GUIContent.none);

            if (maxTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fixed)
            {
                EditorGUILayout.PropertyField(maxFixedProperty, new GUIContent(" "), GUILayout.Width(EditorGUIUtility.labelWidth + FlexalonObjectEditor.ValueWidth + 3));
            }
            else if (maxTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fill)
            {
                EditorGUILayout.PropertyField(maxOfParentProperty, new GUIContent(" "), GUILayout.Width(EditorGUIUtility.labelWidth + FlexalonObjectEditor.ValueWidth + 3));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField(" ", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.TextField("-", GUILayout.Width(FlexalonObjectEditor.ValueWidth));
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(maxTypeProperty, GUIContent.none);

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndHorizontal();
        }

        private void CreateSpacingProperty(string label1, string label2, SerializedProperty property1, SerializedProperty property2)
        {
            EditorGUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUILayout.PropertyField(property1, new GUIContent(label1));
            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(property2, new GUIContent(label2));
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndHorizontal();
        }

        private void CreateSizeProperty2(SerializedProperty typeProperty, SerializedProperty fixedProperty, SerializedProperty ofParentProperty,
            SerializedProperty minTypeProperty, SerializedProperty minFixedProperty, SerializedProperty minOfParentProperty,
            SerializedProperty maxTypeProperty, SerializedProperty maxFixedProperty, SerializedProperty maxOfParentProperty,
            SerializedProperty margin1, SerializedProperty margin2, SerializedProperty padding1, SerializedProperty padding2)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth - 54);
            EditorGUILayout.LabelField("Min/Max", GUILayout.Width(54));
            if (minTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fixed)
            {
                EditorGUILayout.PropertyField(minFixedProperty, GUIContent.none);
            }
            else if (minTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fill)
            {
                EditorGUILayout.PropertyField(minOfParentProperty, GUIContent.none);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("-");
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(minTypeProperty, GUIContent.none);

            if (maxTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fixed)
            {
                EditorGUILayout.PropertyField(maxFixedProperty, GUIContent.none);
            }
            else if (maxTypeProperty.enumValueIndex == (int)MinMaxSizeType.Fill)
            {
                EditorGUILayout.PropertyField(maxOfParentProperty, GUIContent.none);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("-");
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.PropertyField(maxTypeProperty, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth - 54);
            EditorGUILayout.LabelField("Margin", GUILayout.Width(54));
            EditorGUILayout.PropertyField(margin1, GUIContent.none);
            EditorGUILayout.PropertyField(margin2, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIUtility.labelWidth - 54);
            EditorGUILayout.LabelField("Padding", GUILayout.Width(54));
            EditorGUILayout.PropertyField(padding1, GUIContent.none);
            EditorGUILayout.PropertyField(padding2, GUIContent.none);
            EditorGUILayout.EndHorizontal();
        }
        void OnSceneGUI()
        {
            // Draw a box at the transforms position
            var script = this.target as FlexalonObject;
            var node = Runtime.Core.Flexalon.GetNode(script.gameObject);
            if (node == null || node.Result == null)
            {
                return;
            }

            var r = node.Result;

            if (node.Parent != null)
            {
                var layoutBoxScale = node.GetWorldBoxScale(false);
                var layoutRotation = script.transform.parent != null ? script.transform.parent.rotation * r.LayoutRotation : r.LayoutRotation;

                // Box used to layout this object, plus margins.
                Handles.color = new Color(1f, 1f, .2f, 1.0f);
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center + node.Margin.Center, r.RotatedAndScaledBounds.size + node.Margin.Size);

                // Box used to layout this object.
                Handles.color = new Color(.2f, 1f, .5f, 1.0f);
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center, r.RotatedAndScaledBounds.size);
            }

            // Box in which children are layed out. This is the box with handles on it.
            Handles.color = Color.cyan;
            var worldBoxScale = node.GetWorldBoxScale(true);
            Handles.matrix = Matrix4x4.TRS(node.GetWorldBoxPosition(worldBoxScale, false), script.transform.rotation, worldBoxScale);
            Handles.DrawWireCube(Vector3.zero, r.AdapterBounds.size);

            var id = 0;
            float result;
            if (script.WidthType == SizeType.Fixed)
            {
                if (this.CreateSizeHandles(id++, id++, r.AdapterBounds.size, 0, script, out result))
                {
                    this.Record(script);
                    script.Width = result;
                    this.MarkDirty(script);
                }
            }

            if (script.HeightType == SizeType.Fixed)
            {
                if (this.CreateSizeHandles(id++, id++, r.AdapterBounds.size, 1, script, out result))
                {
                    this.Record(script);
                    script.Height = result;
                    this.MarkDirty(script);
                }
            }

            if (script.DepthType == SizeType.Fixed)
            {
                if (this.CreateSizeHandles(id++, id++, r.AdapterBounds.size, 2, script, out result))
                {
                    this.Record(script);
                    script.Depth = result;
                    this.MarkDirty(script);
                }
            }
        }

        private bool CreateSizeHandles(int id1, int id2, Vector3 size, int axis, FlexalonObject script, out float result)
        {
            bool changed = false;
            result = 0;

            if (this.CreateSizeHandleOnSide(id1, size, axis, 1, script, out float r1))
            {
                result = r1;
                changed = true;
            }

            if (this.CreateSizeHandleOnSide(id2, size, axis, -1, script, out float r2))
            {
                result = r2;
                changed = true;
            }

            return changed;
        }

        private bool CreateSizeHandleOnSide(int id, Vector3 size, int axis, int positive, FlexalonObject script, out float result)
        {
            var cid = GUIUtility.GetControlID(id, FocusType.Passive);
            var p = new Vector3();
            p[axis] = size[axis] / 2 * positive;
            EditorGUI.BeginChangeCheck();
#if UNITY_2022_1_OR_NEWER
            Vector3 newPos = Handles.FreeMoveHandle(cid, p, HandleUtility.GetHandleSize(p) * 0.2f, Vector3.one * 0.1f, Handles.SphereHandleCap);
#else
            Vector3 newPos = Handles.FreeMoveHandle(cid, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.2f, Vector3.one * 0.1f, Handles.SphereHandleCap);
#endif
            if (EditorGUI.EndChangeCheck())
            {
                result = newPos[axis] * 2 * positive;
                return true;
            }

            result = 0;
            return false;
        }
    }
}