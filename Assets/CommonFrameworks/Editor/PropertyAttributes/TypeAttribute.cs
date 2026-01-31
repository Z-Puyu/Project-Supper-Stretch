using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CommonFrameworks.Editor.PropertyAttributes {
    public sealed class TypeAttribute : PropertyAttribute {
        private bool AllowsInterface { get; }
        private bool AllowsStruct { get; }
        private bool AllowsAbstractClass { get; }
        private bool AllowsEnum { get; }
        private string TypeExclusionCallback { get; }
        
        public TypeAttribute(TypeFlags flags = TypeFlags.ConcreteClass, string typeExclusionCallback = "") {
            this.AllowsInterface = (flags & TypeFlags.Interface) == TypeFlags.Interface;
            this.AllowsStruct = (flags & TypeFlags.Struct) == TypeFlags.Struct;
            this.AllowsAbstractClass = (flags & TypeFlags.AbstractClass) == TypeFlags.AbstractClass;
            this.AllowsEnum = (flags & TypeFlags.Enum) == TypeFlags.Enum;
            this.TypeExclusionCallback = typeExclusionCallback;
        }

        public TypeAttribute(string typeExclusionCallback) : this(TypeFlags.ConcreteClass, typeExclusionCallback) { }

        private bool Excludes(Type type, SerializedObject obj) {
            return obj.targetObject.GetType().GetMethod(this.TypeExclusionCallback, new[] { typeof(Type) })
                      ?.Invoke(obj.targetObject, new object[] { type }) is true;
        }

        internal bool Allows(Type type, SerializedObject obj) {
            return !this.Excludes(type, obj) &&
                   (!type.IsInterface || this.AllowsInterface) &&
                   (!type.IsValueType || this.AllowsStruct) &&
                   (!type.IsAbstract || this.AllowsAbstractClass) &&
                   (!type.IsEnum || this.AllowsEnum);
        }
    }
}
