using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace GameplayAbilities.Common {
    public sealed class UserData : IUserData {
        private static readonly ObjectPool<UserData> Pool = new ObjectPool<UserData>(
            () => new UserData(), defaultCapacity: 20, maxSize: 100
        );

        private IDictionary<string, double> FloatEntries { get; } = new Dictionary<string, double>();
        private IDictionary<string, int> IntEntries { get; } = new Dictionary<string, int>();
        private ISet<string> Flags { get; } = new HashSet<string>();

        private UserData() { }

        public static UserData New() {
            return UserData.Pool.Get();
        }

        double IUserData.ReadValue(string key) {
            if (this.FloatEntries.TryGetValue(key, out double value)) {
                return value;
            }
            
            return this.IntEntries.TryGetValue(key, out int intValue) ? intValue : 0;
        }
        
        int IUserData.ReadInteger(string key) {
            return this.IntEntries.TryGetValue(key, out int value) ? value : 0;       
        }
        
        bool IUserData.HasFlag(string flag) {
            return this.Flags.Contains(flag);
        }

        public IUserData With(string key, double value) {
            this.FloatEntries[key] = value;
            return this;
        }

        public IUserData With(string key, int value) {
            this.IntEntries[key] = value;
            return this;
        }
        
        public IUserData WithFlag(string flag) {
            this.Flags.Add(flag);
            return this;
        }
        
        public IUserData ExceptValue(string key) {
            this.FloatEntries.Remove(key);
            this.IntEntries.Remove(key);
            return this;
        }
        
        public IUserData ExceptFlag(string flag) {
            this.Flags.Remove(flag);
            return this;       
        }
        
        void IDisposable.Dispose() {
            this.FloatEntries.Clear();
            UserData.Pool.Release(this);
        }
    }
}
