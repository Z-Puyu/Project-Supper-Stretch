using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameplayAbilities.Runtime.EditorTooling {
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class CustomPropertyAttribute : PropertyAttribute { }
}
