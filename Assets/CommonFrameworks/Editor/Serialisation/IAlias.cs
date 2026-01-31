using System.Collections.Generic;

namespace CommonFrameworks.Editor.Serialisation {
    internal interface IAlias<out T> {
        internal IEnumerable<T> Options { get; }
    }
}
