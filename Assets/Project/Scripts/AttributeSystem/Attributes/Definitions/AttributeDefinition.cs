using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[CreateAssetMenu(fileName = "AttributeDefinition", menuName = "Attribute System/Attribute Definition", order = 0)]
public class AttributeDefinition : ScriptableObject {
    [field: SerializeField, Required, DefaultExpand]
    public AttributeTag RootAttribute { get; private set; } = new AttributeTag();
    
    public bool Contains(AttributeKey key, out AttributeTag? tag, bool exact = false) {
        bool contains = false;
        AttributeTag? current = null;
        this.PreorderTraverse(forEach: check, until: _ => contains);
        tag = current;
        return contains;
        
        void check(AttributeTag attribute) {
            contains = attribute.Key == key || (!exact && attribute.Synonyms.Contains(key));
            current = contains ? attribute : current;
        }
    }
    
    public bool Contains(string key, out AttributeTag? tag, bool exact = false) {
        return this.Contains(AttributeKey.OfPath(key), out tag, exact);
    }

    public ICollection<AttributeTag> CollectIf(
        Predicate<AttributeTag> predicate, ICollection<AttributeTag>? collection = null
    ) {
        collection ??= [];
        this.PreorderTraverse(forEach: tryCollect);
        return collection;

        void tryCollect(AttributeTag attribute) {
            if (predicate.Invoke(attribute)) {
                collection.Add(attribute);
            }
        }
    }

    public void PreorderTraverse(
        Action<AttributeTag>? forEach = null, Action<(AttributeTag current, AttributeTag? parent)>? forEachParent = null,
        Func<AttributeTag, bool>? until = null
    ) {
        Dictionary<AttributeTag, AttributeTag> predecessors = [];
        Stack<AttributeTag> stack = [];
        stack.Push(this.RootAttribute);
        while (stack.TryPop(out AttributeTag? current)) {
            if (current.Name != string.Empty) {
                if (current.Key != string.Empty) {
                    forEach?.Invoke(current);
                }
                
                forEachParent?.Invoke(predecessors.TryGetValue(current, out AttributeTag parent)
                        ? (current, parent)
                        : (current, null));
                if (until?.Invoke(current) ?? false) {
                    return;
                }
            }

            for (int i = current.SubAttributes.Count - 1; i >= 0; i -= 1) {
                stack.Push(current.SubAttributes[i]);
                predecessors.Add(current.SubAttributes[i], current);
            }
        }
    }

    public Dictionary<AttributeKey, AttributeTag> ToDictionary() {
        Dictionary<AttributeKey, AttributeTag> attributes = [];
        this.PreorderTraverse(forEach: tag => attributes.Add(tag.Key(), tag));
        return attributes;
    }

    public List<AttributeKey> Fetch() {
        List<AttributeKey> attributes = [];
        this.PreorderTraverse(forEach: tag => attributes.Add(tag.Key));
        return attributes;
    }

    public AdvancedDropdownList<AttributeKey> FetchLeaves() {
        Dictionary<AttributeKey, AdvancedDropdownList<AttributeKey>> sections = [];
        this.PreorderTraverse(forEach: makeSection, forEachParent: linkSections);

        return sections[this.RootAttribute.Key];

        void makeSection(AttributeTag tag) {
            AdvancedDropdownList<AttributeKey> section = tag.SubAttributes.Count > 0
                    ? new AdvancedDropdownList<AttributeKey>(tag.Name)
                    : new AdvancedDropdownList<AttributeKey>(tag.Name, tag.Key);
            sections.TryAdd(tag.Key, section);
        }

        void linkSections((AttributeTag current, AttributeTag? parent) node) {
            if (node.parent is not null) {
                sections[node.parent.Key].Add(sections[node.current.Key]);
            }
        }
    }
}
