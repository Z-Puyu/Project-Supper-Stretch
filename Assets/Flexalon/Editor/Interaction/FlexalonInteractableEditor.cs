#if UNITY_PHYSICS

using Flexalon.Runtime.Interaction;
using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor.Interaction
{
    [CustomEditor(typeof(FlexalonInteractable)), CanEditMultipleObjects]
    public class FlexalonInteractableEditor : UnityEditor.Editor
    {
        private SerializedProperty _clickable;
        private SerializedProperty _maxClickTime;
        private SerializedProperty _maxClickDistance;
        private SerializedProperty _draggable;
        private SerializedProperty _interpolationSpeed;
        private SerializedProperty _insertRadius;
        private SerializedProperty _restriction;
        private SerializedProperty _planeNormal;
        private SerializedProperty _localSpaceRestriction;
        private SerializedProperty _lineDirection;
        private SerializedProperty _holdOffset;
        private SerializedProperty _localSpaceOffset;
        private SerializedProperty _rotateOnDrag;
        private SerializedProperty _holdRotation;
        private SerializedProperty _localSpaceRotation;
        private SerializedProperty _hideCursor;
        private SerializedProperty _handle;
        private SerializedProperty _bounds;
        private SerializedProperty _layerMask;
        private SerializedProperty _clicked;
        private SerializedProperty _hoverStart;
        private SerializedProperty _hoverEnd;
        private SerializedProperty _selectStart;
        private SerializedProperty _selectEnd;
        private SerializedProperty _dragStart;
        private SerializedProperty _dragEnd;

        private static bool _showDragOptions = true;
        private static bool _showEvents = false;

        void OnEnable()
        {
            this._clickable = this.serializedObject.FindProperty("_clickable");
            this._maxClickTime = this.serializedObject.FindProperty("_maxClickTime");
            this._maxClickDistance = this.serializedObject.FindProperty("_maxClickDistance");
            this._draggable = this.serializedObject.FindProperty("_draggable");
            this._interpolationSpeed = this.serializedObject.FindProperty("_interpolationSpeed");
            this._insertRadius = this.serializedObject.FindProperty("_insertRadius");
            this._restriction = this.serializedObject.FindProperty("_restriction");
            this._planeNormal = this.serializedObject.FindProperty("_planeNormal");
            this._localSpaceRestriction = this.serializedObject.FindProperty("_localSpaceRestriction");
            this._lineDirection = this.serializedObject.FindProperty("_lineDirection");
            this._holdOffset = this.serializedObject.FindProperty("_holdOffset");
            this._localSpaceOffset = this.serializedObject.FindProperty("_localSpaceOffset");
            this._rotateOnDrag = this.serializedObject.FindProperty("_rotateOnDrag");
            this._holdRotation = this.serializedObject.FindProperty("_holdRotation");
            this._localSpaceRotation = this.serializedObject.FindProperty("_localSpaceRotation");
            this._hideCursor = this.serializedObject.FindProperty("_hideCursor");
            this._handle = this.serializedObject.FindProperty("_handle");
            this._bounds = this.serializedObject.FindProperty("_bounds");
            this._layerMask = this.serializedObject.FindProperty("_layerMask");
            this._clicked = this.serializedObject.FindProperty("_clicked");
            this._hoverStart = this.serializedObject.FindProperty("_hoverStart");
            this._hoverEnd = this.serializedObject.FindProperty("_hoverEnd");
            this._selectStart = this.serializedObject.FindProperty("_selectStart");
            this._selectEnd = this.serializedObject.FindProperty("_selectEnd");
            this._dragStart = this.serializedObject.FindProperty("_dragStart");
            this._dragEnd = this.serializedObject.FindProperty("_dragEnd");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this._clickable);

            if (this._clickable.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(this._maxClickTime);
                EditorGUILayout.PropertyField(this._maxClickDistance);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(this._draggable);

            if (this._draggable.boolValue)
            {
                EditorGUILayout.Space();
                FlexalonInteractableEditor._showDragOptions = EditorGUILayout.Foldout(FlexalonInteractableEditor._showDragOptions, "Drag Options");
                if (FlexalonInteractableEditor._showDragOptions)
                {
                    EditorGUI.indentLevel++;

                    bool showAllOptions = true;
                    foreach (var target in this.targets)
                    {
                        var interactable = target as FlexalonInteractable;
                        showAllOptions = showAllOptions && interactable._showAllDragProperties;
                    }

                    if (showAllOptions)
                    {
                        EditorGUILayout.PropertyField(this._interpolationSpeed);
                    }

                    EditorGUILayout.PropertyField(this._insertRadius);

                    if (showAllOptions)
                    {
                        var restriction = this._restriction;
                        EditorGUILayout.PropertyField(restriction);
                        if (restriction.enumValueIndex == (int)FlexalonInteractable.RestrictionType.Plane)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(this._planeNormal, new GUIContent("Normal"));
                            EditorGUILayout.PropertyField(this._localSpaceRestriction, new GUIContent("Local Space"));
                            EditorGUI.indentLevel--;
                        }
                        else if (restriction.enumValueIndex == (int)FlexalonInteractable.RestrictionType.Line)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(this._lineDirection, new GUIContent("Direction"));
                            EditorGUILayout.PropertyField(this._localSpaceRestriction, new GUIContent("Local Space"));
                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.PropertyField(this._holdOffset);
                        EditorGUILayout.PropertyField(this._localSpaceOffset, new GUIContent("Local Space"));

                        var rotateOnGrab = this._rotateOnDrag;
                        EditorGUILayout.PropertyField(rotateOnGrab);
                        if (rotateOnGrab.boolValue)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(this._holdRotation, new GUIContent("Rotation"));
                            EditorGUILayout.PropertyField(this._localSpaceRotation, new GUIContent("Local Space"));
                            EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.PropertyField(this._handle);
                        EditorGUILayout.PropertyField(this._bounds);
                    }
                    else
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Some drag options are disabled for the selected input provider.", MessageType.Info);
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.PropertyField(this._hideCursor);
                    EditorGUILayout.PropertyField(this._layerMask);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space();

            FlexalonInteractableEditor._showEvents = EditorGUILayout.Foldout(FlexalonInteractableEditor._showEvents, "Events");
            if (FlexalonInteractableEditor._showEvents)
            {
                if (this._clickable.boolValue)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_clicked"));
                }

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_hoverStart"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_hoverEnd"));

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_selectStart"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_selectEnd"));

                if (this._draggable.boolValue)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_dragStart"));
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_dragEnd"));
                }
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif