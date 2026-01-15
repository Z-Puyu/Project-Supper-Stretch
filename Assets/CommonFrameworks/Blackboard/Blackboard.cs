using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace CommonFrameworks.Blackboard {
    [Serializable]
    public sealed class Blackboard {
        private bool IsInitialised { get; set; }
        private IDictionary<Type, object> Segments { get; } = new Dictionary<Type, object>();
        
        [field: SerializeField] 
        private SaintsDictionary<string, DynamicValue> InitialValues { get; set; } =
            new SaintsDictionary<string, DynamicValue>();

        private void Initialise() {
            foreach ((string key, DynamicValue value) in this.InitialValues) {
                switch (value.DataType) {
                    case DynamicValue.Type.Bool:
                        if (!this.Segments.TryGetValue(typeof(bool), out object segment)) {
                            segment = new BlackboardSegment<bool>();
                            this.Segments[typeof(bool)] = segment;
                        } 
                        
                        ((BlackboardSegment<bool>)segment).Set(key, value);
                        break;
                    case DynamicValue.Type.Int:
                        if (!this.Segments.TryGetValue(typeof(int), out segment)) {
                            segment = new BlackboardSegment<int>();
                            this.Segments[typeof(int)] = segment;
                        }
                        
                        ((BlackboardSegment<int>)segment).Set(key, value);
                        break;
                    case DynamicValue.Type.Float:
                        if (!this.Segments.TryGetValue(typeof(double), out segment)) {
                            segment = new BlackboardSegment<double>();
                            this.Segments[typeof(double)] = segment;
                        }
                        
                        ((BlackboardSegment<double>)segment).Set(key, value);
                        break;
                    case DynamicValue.Type.String:
                        if (!this.Segments.TryGetValue(typeof(string), out segment)) {
                            segment = new BlackboardSegment<string>();
                            this.Segments[typeof(string)] = segment;
                        }
                        
                        ((BlackboardSegment<string>)segment).Set(key, value);
                        break;
                }
            }

            this.IsInitialised = true;
        }
        
        private BlackboardSegment<T> FindSegment<T>() {
            if (!this.IsInitialised) {
                this.Initialise();
            }
            
            if (this.Segments.TryGetValue(typeof(T), out object segment)) {
                return (BlackboardSegment<T>)segment;
            }

            segment = new BlackboardSegment<T>();
            this.Segments[typeof(T)] = segment;
            return (BlackboardSegment<T>)segment;
        }
        
        public bool TryGetValue<T>(BlackboardKey key, out T value) {
            BlackboardSegment<T> segment = this.FindSegment<T>();
            return segment.HasValue(key, out value);
        }
        
        public void Set<T>(BlackboardKey key, T value) {
            BlackboardSegment<T> segment = this.FindSegment<T>();
            segment.Set(key, value);
        }
        
        private sealed class BlackboardSegment<T> {
            private IDictionary<BlackboardKey, BlackboardEntry<T>> Entries { get; } =
                new Dictionary<BlackboardKey, BlackboardEntry<T>>();
        
            internal bool HasValue(BlackboardKey key, out T value) {
                if (this.Entries.TryGetValue(key, out BlackboardEntry<T> entry)) {
                    value = entry.Value;
                    return true;
                }
            
                value = default!;
                return false;
            }

            internal void Set(BlackboardKey key, T value) {
                this.Entries[key] = new BlackboardEntry<T>(value);
            }
        }
    }
}
