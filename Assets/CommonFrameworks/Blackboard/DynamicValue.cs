using System;

namespace CommonFrameworks.Blackboard {
    public struct DynamicValue {
        public enum Type {
            Bool,
            Int,
            Float,
            String
        }
    
        private Type ValueType { get; }
        private double NumericValue { get; }
        private string StringValue { get; }

        public T As<T>() {
            return this.ValueType switch {
                Type.Bool => this.AsBool<T>()!,
                Type.Float => this.AsFloat<T>()!,
                Type.Int => this.AsInteger<T>()!,
                Type.String when typeof(T) == typeof(string) => (this.StringValue is T t ? t : default)!,
                var _ => throw new NotSupportedException($"{typeof(T)} is not a supported type!")
            };
        }

        private T? AsBool<T>() {
            bool value = this.NumericValue != 0;
            return typeof(T) == typeof(bool) && value is T t ? t : default;
        }

        private T? AsFloat<T>() {
            if (typeof(T) == typeof(float)) {
                float f = (float)this.NumericValue;
                return f is T t ? t : default;
            }

            if (typeof(T) == typeof(double)) {
                return this.NumericValue is T t ? t : default;
            }
        
            return default;
        }

        private T? AsInteger<T>() {
            if (typeof(T) == typeof(int)) {
                int i = (int)this.NumericValue;
                return i is T t ? t : default;
            }

            if (typeof(T) == typeof(long)) {
                long l = (long)this.NumericValue;
                return l is T t ? t : default;
            }
        
            return default;
        }

        public static implicit operator bool(DynamicValue value) {
            return value.NumericValue != 0;
        }
    
        public static implicit operator float(DynamicValue value) {
            return (float)value.NumericValue;
        }

        public static implicit operator double(DynamicValue value) {
            return value.NumericValue;
        }

        public static implicit operator int(DynamicValue value) {
            return (int)value.NumericValue;
        }

        public static implicit operator long(DynamicValue value) {
            return (long)value.NumericValue;
        }

        public static implicit operator string(DynamicValue value) {
            return value.StringValue;
        }
    }
}
