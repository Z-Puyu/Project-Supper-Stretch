using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UI {
    [Serializable]
    public struct VisualElementIdentifier {
        internal string Name { get; private set; }
        internal string[] Classes { get; private set; }
        
        internal VisualElementIdentifier(VisualElement element, VisualElement? root = null) {
            ICollection<string> path = new List<string>();
            VisualElement? curr = element;
            while (curr != root && curr is not null) {
                if (!string.IsNullOrWhiteSpace(curr.name)) {
                    path.Add(curr.name);
                }
                
                curr = curr.parent;
            }
            
            this.Name = string.Join('/', path.Reverse());
            this.Classes = element.GetClasses().ToArray();
        }

        internal static VisualElementIdentifier Parse(string id) {
            string[] tokens = id.Split(' ');
            return new VisualElementIdentifier {
                Name = tokens[0],
                Classes = tokens[1][1..^1].Split(' ').Select(@class => @class[1..]).ToArray()
            };
        }

        public override string ToString() {
            return this.Classes.Length == 0
                    ? this.Name
                    : $"{this.Name} ({string.Join(' ', this.Classes.Select(@class => $".{@class}"))})";
        }
    }
}
