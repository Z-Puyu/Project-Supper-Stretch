using System;
using SaintsField.Playa;
using UnityEngine;

namespace CommonFrameworks.Blackboard {
    [Serializable]
    internal record struct DynamicValue {
        internal enum Type {
            Bool,
            Int,
            Float,
            String
        }
    
        internal Type DataType { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(this.DataType), Type.Bool)]
        private bool BoolValue { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.DataType), Type.Int)]
        private int IntValue { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.DataType), Type.Float)]
        private double FloatValue { get; set; }
        
        [field: SerializeField, ShowIf(nameof(this.DataType), Type.String)]
        private string StringValue { get; set; }

        public T As<T>() {
            return this.DataType switch {
                Type.Bool => this.AsBool<T>(),
                Type.Float => this.AsFloat<T>(),
                Type.Int => this.AsInteger<T>(),
                Type.String when typeof(T) == typeof(string) => this.StringValue is T t ? t : default!,
                var _ => throw new NotSupportedException($"{typeof(T)} is not a supported type!")
            };
        }

        private T AsBool<T>() {
            return typeof(T) == typeof(bool) && this.BoolValue is T t ? t : default!;
        }

        private T AsFloat<T>() {
            if (typeof(T) == typeof(float)) {
                return (float)this.FloatValue is T t ? t : default!;
            }

            if (typeof(T) == typeof(double)) {
                return this.FloatValue is T t ? t : default!;
            }
        
            return default!;
        }

        private T AsInteger<T>() {
            if (typeof(T) == typeof(int)) {
                return this.IntValue is T t ? t : default!;
            }

            if (typeof(T) == typeof(long)) {
                return (long)this.IntValue is T t ? t : default!;
            }
            
            if (typeof(T) == typeof(short)) {
                return (short)this.IntValue is T t ? t : default!;
            }

            if (typeof(T) == typeof(byte)) {
                return (byte)this.IntValue is T t ? t : default!;
            }

            if (typeof(T) == typeof(sbyte)) {
                return (sbyte)this.IntValue is T t ? t : default!;
            }

            if (typeof(T) == typeof(uint)) {
                return (uint)this.IntValue is T t ? t : default!;
            }
            
            if (typeof(T) == typeof(ulong)) {
                return (ulong)this.IntValue is T t ? t : default!;
            }
            
            if (typeof(T) == typeof(ushort)) {
                return (ushort)this.IntValue is T t ? t : default!;
            }
        
            return default!;
        }

        public static implicit operator bool(DynamicValue value) {
            return value is { DataType: Type.Bool, BoolValue: true };
        }
    
        public static implicit operator float(DynamicValue value) {
            return value.DataType == Type.Float ? (float)value.FloatValue : 0;
        }

        public static implicit operator double(DynamicValue value) {
            return value.DataType == Type.Float ? value.FloatValue : 0;
        }

        public static implicit operator int(DynamicValue value) {
            return value.DataType == Type.Int ? value.IntValue : 0;
        }

        public static implicit operator long(DynamicValue value) {
            return value.DataType == Type.Int ? (long)value.IntValue : 0;
        }

        public static implicit operator string(DynamicValue value) {
            return value.DataType == Type.String ? value.StringValue : string.Empty;
        }
    }
}
