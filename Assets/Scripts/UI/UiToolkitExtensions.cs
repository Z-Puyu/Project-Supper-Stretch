using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UI {
    public static class UiToolkitExtensions {
        public static List<V> FetchNamedChildren<V>(this VisualElement root) where V : VisualElement {
            return root.Query<V>().Where(child => !string.IsNullOrWhiteSpace(child.name)).ToList();
        }
        
        public static List<VisualElement> FetchNamedChildren(this VisualElement root) {
            return root.FetchNamedChildren<VisualElement>();
        }
    }
}
