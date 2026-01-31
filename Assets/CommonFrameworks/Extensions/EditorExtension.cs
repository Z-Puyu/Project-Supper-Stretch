using System;
using CommonFrameworks.Collections;
using SaintsField;

namespace CommonFrameworks.Extensions {
    public static class EditorExtension {
        public static AdvancedDropdownList<S> Collate<T, S>(this ITree<T, S> tree, Func<T, S?, string>? descriptor = null) {
            descriptor ??= (item, data) => data is null ? $"{item}" : $"{data}";
            return tree.Aggregate<AdvancedDropdownList<S>>(
                combiner: (item, data, sublists) => new AdvancedDropdownList<S>(descriptor(item, data), sublists),
                tree.Root,
                synthesiser: (vertex, data) => data is null
                        ? new AdvancedDropdownList<S>(descriptor(vertex, data))
                        : new AdvancedDropdownList<S>(descriptor(vertex, data), data)
            );
        }
    }
}
