using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameplayAbilities.Editor {
    public class SubtypeDropdownMenu : AdvancedDropdown {
        private readonly List<Type> types;
        internal event Action<Type?> OnSelected = delegate { };

        public SubtypeDropdownMenu(AdvancedDropdownState state, List<Type> types) : base(state) {
            this.types = types;
            this.minimumSize = new Vector2(250, 300);
        }

        private static bool IsUserDefinedAssembly(Assembly assembly) {
            string name = assembly.GetName().Name;
            return name == "Assembly-CSharp" || name == "Assembly-CSharp-Editor" || name.StartsWith("UnityEngine.") ||
                   name.StartsWith("UnityEditor.") || name.StartsWith("System.");
        }

        protected override AdvancedDropdownItem BuildRoot() {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Subtypes");

            // Add 'None' option
            root.AddChild(new Item(null));
            root.AddSeparator();

            foreach (Type? type in this.types) {
                // Split by namespace to create a hierarchy
                string path = type.FullName ?? type.Name;
                string[] sections = path.Split('.');

                AdvancedDropdownItem submenu = root;
                if (SubtypeDropdownMenu.IsUserDefinedAssembly(type.Assembly)) {
                    AdvancedDropdownItem category = new AdvancedDropdownItem(type.Assembly.GetName().Name);
                    submenu.AddChild(category);
                    submenu = category;
                }

                for (int i = 0; i < sections.Length - 1; i += 1) {
                    AdvancedDropdownItem? existing = submenu.children.FirstOrDefault(c => c.name == sections[i]);
                    if (existing is not null) {
                        submenu = existing;
                    } else {
                        AdvancedDropdownItem menu = new AdvancedDropdownItem(sections[i]);
                        submenu.AddChild(menu);
                        submenu = menu;
                    }
                }

                submenu.AddChild(new Item(type));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            if (item is Item type) {
                this.OnSelected.Invoke(type.Type);
            }
        }

        private class Item : AdvancedDropdownItem {
            internal Type? Type { get; }

            internal Item(Type? type) : base(
                ObjectNames.NicifyVariableName(
                    type is null ? "null" : $"{type.Name} ({type.Assembly.GetName().Name}.{type.Namespace})"
                )
            ) {
                this.Type = type;
            }
        }
    }
}