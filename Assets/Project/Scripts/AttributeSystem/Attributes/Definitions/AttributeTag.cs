using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using Editor;
#endif
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[Serializable]
public class AttributeTag {
#if UNITY_EDITOR
    private AdvancedDropdownList<AttributeKey> AllDefinitions => AttributeKey.AllDefinitions;

    private AdvancedDropdownList<AttributeKey> AllSameGroupDefinitions => this.Key == string.Empty 
            ? [] 
            : ObjectCache<AttributeDefinition>.Instance.Objects
                                              .First(def => def.Contains(this.Key, out AttributeTag _, exact: true))
                                              .FetchLeaves();

    private void HandleRename() {
        this.Key = this.Key();
    }
    
    private void HandleSynonymChange(object _, int idx = -1) {
        if (idx < 0 || idx >= this.Synonyms.Count) {
            return;
        }

        AttributeTag tag = this.Synonyms[idx].Attribute();
        if (!tag.Synonyms.Contains(this.Key)) {
            tag.Synonyms.Add(this.Key);
        }
    }

    private void HandleSynonymCountChange() {
        IEnumerable<AttributeTag> synonyms = this.Synonyms
                                                 .Select(synonym => synonym.Attribute())
                                                 .Where(tag => !tag.Synonyms.Contains(this.Key));
        foreach (AttributeTag tag in synonyms) {
            if (!tag.Synonyms.Contains(this.Key)) {
                tag.Synonyms.Add(this.Key);
            }
        }
    }

    private void HandleAntonymChange(object _, int idx = -1) {
        if (idx < 0 || idx >= this.Antonyms.Count) {
            return;
        }
        
        AttributeTag tag = this.Antonyms[idx].Attribute();
        if (!tag.Antonyms.Contains(this.Key)) {
            tag.Antonyms.Add(this.Key);
        }
    }

    private void HandleAntonymCountChange() {
        IEnumerable<AttributeTag> antonyms = this.Antonyms
                                                 .Select(antonym => antonym.Attribute())
                                                 .Where(tag => !tag.Antonyms.Contains(this.Key));
        foreach (AttributeTag tag in antonyms) {
            if (!tag.Antonyms.Contains(this.Key)) {
                tag.Antonyms.Add(this.Key);
            }
        }
    }
#endif
    public enum ClampPolicy { None, CapByAttribute, CapByValue }

    [field: SerializeField, ReadOnly, DefaultExpand]
    public AttributeKey Key { get; private set; } = "NewAttribute";
    
    [field: SerializeField, Required, OnValueChanged(nameof(this.HandleRename))]
    public string Name { get; private set; } = "NewAttribute";
    
    [field: SerializeField, ShowIf(nameof(this.IsLeaf))] 
    public ClampPolicy HowToClamp { get; private set; } = ClampPolicy.None;
    
    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByAttribute)]
    [field: AdvancedDropdown(nameof(this.AllSameGroupDefinitions))]
    public AttributeKey Cap { get; private set; }

    [field: SerializeField, ShowIf(nameof(this.HowToClamp), ClampPolicy.CapByValue)]
    public int MaxValue { get; private set; }

    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))]
    [field: OnValueChanged(nameof(this.HandleSynonymChange)), OnArraySizeChanged(nameof(this.HandleSynonymChange))]
    public List<AttributeKey> Synonyms { get; private set; } = [];

    [field: SerializeField, AdvancedDropdown(nameof(this.AllDefinitions))] 
    [field: OnValueChanged(nameof(this.HandleAntonymChange)), OnArraySizeChanged(nameof(this.HandleAntonymChange))]
    public List<AttributeKey> Antonyms { get; private set; } = [];

    [field: SerializeField] public List<AttributeTag> SubAttributes { get; private set; } = [];
    
    private bool IsLeaf => this.SubAttributes.Count == 0;

    public Attribute Zero => this.HowToClamp switch {
        ClampPolicy.None => new Attribute(this.Key, 0, 0),
        ClampPolicy.CapByAttribute => Attribute.WithMaxAttribute(this.Key, 0, this.Cap),
        ClampPolicy.CapByValue => Attribute.WithMaxValue(this.Key, 0, this.MaxValue),
        var _ => throw new ArgumentOutOfRangeException()
    };

    public IEnumerable<AttributeKey> SameSetSynonyms =>
            this.Synonyms.Where(synonym => synonym.FullName.StartsWith(this.Key.Root));

    public static AttributeTag Of(AttributeKey key) {
        return new AttributeTag { Key = key, Name = key.Name };
    }

    public bool Contains(string attribute) {
        return this.Name == attribute || this.SubAttributes.Any(sub => sub.Contains(attribute));
    }

    public override string ToString() {
        return this.Name;   
    }
}
