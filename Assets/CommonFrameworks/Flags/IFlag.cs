using System.Collections.Generic;

namespace CommonFrameworks.Flags {
    public interface IFlag<T> {
        public bool Has(T flags);
        public void Set(T item);
        public void Unset(T item);
        public void Toggle(T item);
        public IEnumerable<T> GetAllPresent();
        public bool HasAnyPresent(out T first);
    }
}

