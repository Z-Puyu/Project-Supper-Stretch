using System.Collections.Generic;
using System.Linq;
using Editor;
using Project.Scripts.AttributeSystem.Modifiers;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

public static class AttributeDefinitionUtils {
    public static AdvancedDropdownList<AttributeKey> FetchOnlyLeaves(this IEnumerable<AttributeDefinition> defs) {
        return new AdvancedDropdownList<AttributeKey>("Attributes", defs.Select(def => def.FetchLeaves()));
    }
    
    public static AdvancedDropdownList<AttributeKey> FetchAll(this IEnumerable<AttributeDefinition> defs) {
        AdvancedDropdownList<AttributeKey> list = new AdvancedDropdownList<AttributeKey>("Attributes");
        foreach (AttributeDefinition def in defs) {
            AdvancedDropdownList<AttributeKey> section = new AdvancedDropdownList<AttributeKey>(def.RootAttribute.Name);
            list.Add(section);
            foreach (AttributeKey key in def.Fetch()) {
                section.Add(key.ToString(), key);
            }       
        }
        
        return list;       
    }

    public static AdvancedDropdownList<ModifierKey> FetchAllModifiers(this IEnumerable<AttributeDefinition> defs) {
        AdvancedDropdownList<ModifierKey> list = new AdvancedDropdownList<ModifierKey>("Modifier Types");
        AdvancedDropdownList<ModifierKey> parent = list;
        foreach (AttributeDefinition def in defs) {
            AdvancedDropdownList<ModifierKey> section = new AdvancedDropdownList<ModifierKey>(def.RootAttribute.Name);
            parent.Add(section);
            AdvancedDropdownList<ModifierKey> current = parent;
            def.PreorderTraverse(forEach: generateEntries);
            parent = section;
            continue;

            void generateEntries(AttributeTag tag) {
                foreach (AttributeTag attribute in tag.SubAttributes) {
                    current.Add(new AdvancedDropdownList<ModifierKey>(attribute.Name));
                }

                ModifierKey modifier = ModifierKey.Of(tag.Key, ModifierType.Base, ModifierOperation.Offset);
                current.Add(modifier.ToString(), modifier);
                modifier = ModifierKey.Of(tag.Key, ModifierType.Base, ModifierOperation.Multiplier);
                current.Add(modifier.ToString(), modifier);
                modifier = ModifierKey.Of(tag.Key, ModifierType.Current, ModifierOperation.Offset);
                current.Add(modifier.ToString(), modifier);
                modifier = ModifierKey.Of(tag.Key, ModifierType.Current, ModifierOperation.Multiplier);
                current.Add(modifier.ToString(), modifier);
            }
        }

        return list;
    }
    
    public static string Path(this AttributeTag tag) {
        Dictionary<AttributeTag, AttributeTag> parents = [];
        Stack<string> parts = [];
        string path = string.Empty;
        foreach (AttributeDefinition def in ObjectCache<AttributeDefinition>.Instance.Objects) {
            def.PreorderTraverse(forEachParent: tryUpdate, until: _ => path != string.Empty);
            if (path != string.Empty) {
                break;
            }       
        }
        
        return path;

        void tryUpdate((AttributeTag current, AttributeTag? parent) node) {
            Debug.Log($"{node.parent?.Name} -> {node.current.Name}");
            if (node.parent is not null) {
                parents.TryAdd(node.current, node.parent);
            }

            if (node.current != tag) {
                return;
            }

            AttributeTag curr = node.current;
            parts.Push(curr.Name);
            while (parents.TryGetValue(curr, out AttributeTag parent)) {
                parts.Push(parent.Name);
                curr = parent;
            }
                
            path = string.Join('.', parts);
        }
    }
    
    public static AttributeKey Key(this AttributeTag tag) {
        return tag.Path();
    }

    public static AttributeTag Attribute(this AttributeKey key) {
        AttributeTag? tag = null;
        foreach (AttributeDefinition def in ObjectCache<AttributeDefinition>.Instance.Objects) {
            def.PreorderTraverse(forEach: tryUpdate, until: _ => tag is not null);
            if (tag is not null) {
                break;
            }       
        }
        
        return tag ?? AttributeTag.Of(key);
        void tryUpdate(AttributeTag attribute) => tag = attribute.Key == key ? attribute : tag;
    }
}
