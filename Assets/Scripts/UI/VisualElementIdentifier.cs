using System;
using System.Linq;
using UnityEngine.UIElements;

namespace UI {
    [Serializable]
    public struct VisualElementIdentifier {
        internal string Name { get; private set; }
        internal string[] Classes { get; private set; }
        
        internal VisualElementIdentifier(VisualElement element) {
            this.Name = element.name;
            this.Classes = element.GetClasses().ToArray();
        }

        internal static VisualElementIdentifier Parse(string id) {
            string[] tokens = id.Split(' ');
            return new VisualElementIdentifier {
                Name = tokens[0],
                Classes = tokens[1][1..^2].Split(' ').Select(@class => @class[1..]).ToArray()
            };
        }

        public override string ToString() {
            return this.Classes.Length == 0
                    ? this.Name
                    : $"{this.Name} ({string.Join(' ', this.Classes.Select(@class => $".{@class}"))})";
        }
    }
}
