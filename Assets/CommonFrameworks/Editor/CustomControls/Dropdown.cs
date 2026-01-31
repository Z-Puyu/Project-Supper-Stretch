using System.Collections.Generic;
using UnityEngine.UIElements;

namespace CommonFrameworks.Editor.CustomControls {
    internal class Dropdown : CommonControl {
        private List<string> Items { get; set; } = new List<string>();
        
        public Dropdown() {
            this.Q(className: BasePopupField<string, string>.arrowUssClassName).RegisterCallback<PointerDownEvent>(this.OnPointerDown);
        }

        private void OnPointerDown(PointerDownEvent e) {
            
        }

        internal void Add(string option, object item) {
            
        }
    }
}
