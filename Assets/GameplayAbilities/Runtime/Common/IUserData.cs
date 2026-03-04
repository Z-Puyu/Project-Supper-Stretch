using System;

namespace GameplayAbilities.Common {
    public interface IUserData : IDisposable {
        internal double ReadValue(string key);
        internal int ReadInteger(string key);
        internal bool HasFlag(string flag);
        public IUserData With(string key, double value);
        public IUserData With(string key, int value);
        public IUserData WithFlag(string flag);
        public IUserData ExceptValue(string key); 
        public IUserData ExceptFlag(string flag);
    }
}
