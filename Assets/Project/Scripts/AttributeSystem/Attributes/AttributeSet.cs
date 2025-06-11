using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.DataPersistence;
using Project.Scripts.Util.Visitor;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Attributes;

[DisallowMultipleComponent]
public class AttributeSet : MonoBehaviour, IEnumerable<Attribute>, IAttributeReader, IPersistent, IVisitable<AttributeSet> {
    public Guid Id { get; private set; }
    private Dictionary<Enum, Attribute> Attributes { get; init; } = [];
    private Dictionary<Enum, Vector4> Modifiers { get; init; } = [];

    [field: SerializeField, ReadOnly]
    public string Identifier { get; set; }

    public event UnityAction<AttributeSet, Attribute, Attribute> OnAttributeChanged = delegate { };

    /// <summary>
    /// Returns the attribute data with the given type. If the attribute set does not contain this attribute,
    /// try to compute it from the modifiers.
    /// </summary>
    /// <param name="key">The attribute to query.</param>
    public Attribute this[Enum key] {
        get {
            Debug.Log($"Querying for {key}, current attributes: {this}");
            if (this.Attributes.TryGetValue(key, out Attribute val)) {
                return val;
            }

            val = Attribute.Zero(key);
            this.Attributes.Add(key, val);
            return val;
        }
    }

    private AttributeSet() {
        this.Id = Guid.NewGuid();
        this.Identifier = this.Id.ToString();
    }

    private void Awake() {
        this.Id = Guid.Parse(this.Identifier);
    }

    public void Init(Enum attribute, int value, Enum? cap = null, int maxValue = -1) {
        if (maxValue >= 0) {
            this.Attributes.Add(attribute, new Attribute(attribute, value, maxValue));
        } else if (cap is not null) {
            this.Attributes.Add(attribute, new Attribute(attribute, value, cap));
        } else {
            this.Attributes.Add(attribute, new Attribute(attribute, value));
        }

        Debug.Log($"Initialised attribute {attribute} = {value} on {this.gameObject}");
    }

    /// <summary>
    /// Update the current value of the attribute. Called after every modifier change.
    /// </summary>
    /// <param name="attribute">The attribute to recompute.</param>
    private void Recompute(Enum attribute) {
        if (!this.Attributes.TryGetValue(attribute, out Attribute data)) {
            Debug.Log($"Create zero attribute for {attribute}");
            data = Attribute.Zero(attribute);
        }

        if (this.Modifiers.TryGetValue(attribute, out Vector4 m)) {
            Debug.Log($"Modifier for {attribute}: {m}");
            float modified = ((data.BaseValue + m.x) * (1 + m.y / 100) + m.z) * (1 + m.w / 100);
            data = data with { CurrentValue = Mathf.CeilToInt(modified) };
        }

        Debug.Log($"Recomputed {attribute} = {data}");
        this.UpdateAttribute(in data);
    }

    public void AddModifier(Modifier modifier) {
        Debug.Log($"{modifier} to {this.gameObject}");
        if (!this.Modifiers.ContainsKey(modifier.Target)) {
            this.Modifiers.Add(modifier.Target, modifier.ToVector4());
        } else {
            this.Modifiers[modifier.Target] += modifier.ToVector4();
        }

        this.Recompute(modifier.Target);
    }

    /// <summary>
    /// Prepare an attribute update.
    /// </summary>
    /// <param name="newData">The new attribute data.
    /// The current attribute data is not yet replaced with this new data.</param>
    protected virtual void PreAttributeUpdate(Attribute newData) { }

    /// <summary>
    /// Finalise an attribute update. This is the only place where an attribute is updated.
    /// </summary>
    /// <param name="newData">The new attribute data.</param>
    private void UpdateAttribute(in Attribute newData) {
        Attribute old = this[newData.Type];
        Attribute updated = newData;
        this.PreAttributeUpdate(newData);
        if (newData.Cap is not null) {
            int max = this[newData.Cap].CurrentValue;
            updated = newData with { CurrentValue = Mathf.Clamp(newData.CurrentValue, 0, max) };
        } else if (newData.HardLimit >= 0) {
            updated = newData with { CurrentValue = Mathf.Clamp(newData.CurrentValue, 0, newData.HardLimit) };
        }

        Debug.Log($"Pre-update: {this} ({this.Attributes.Count} attributes)");
        this.Attributes[updated.Type] = updated;
        this.PostAttributeUpdate(updated);
        this.OnAttributeChanged.Invoke(this, old, updated);
        Debug.Log($"Post-update: {this} ({this.Attributes.Count} attributes)");
    }

    /// <summary>
    /// Perform any actions after an attribute has been updated.
    /// </summary>
    /// <param name="updated">The updated attribute data. It is the current attribute data.</param>
    protected virtual void PostAttributeUpdate(Attribute updated) { }

    public Attribute Read(Enum attribute) {
        return this[attribute];
    }

    public int ReadCurrent(Enum attribute) {
        return this[attribute].CurrentValue;
    }

    public int ReadBase(Enum attribute) {
        return this[attribute].BaseValue;
    }

    public int ReadMax(Enum attribute) {
        Attribute value = this[attribute];
        if (value.Cap is null) {
            return value.HardLimit >= 0 ? value.HardLimit : int.MaxValue;
        }

        return this[value.Cap].CurrentValue;
    }

    public void Accept(IVisitor<AttributeSet> visitor) {
        visitor.Visit(this);
    }

    public IEnumerator<Attribute> GetEnumerator() {
        return this.Attributes.Values.GetEnumerator();
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder(this.Attributes.Count);
        sb.AppendJoin(' ', this.Attributes.Values);
        return sb.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }

    public Momento Save() {
        ReadOnlyDictionary<Enum, Attribute> attributes = new ReadOnlyDictionary<Enum, Attribute>(this.Attributes);
        ReadOnlyDictionary<Enum, Vector4> modifiers = new ReadOnlyDictionary<Enum, Vector4>(this.Modifiers);
        return new SaveData(this.Id, attributes, modifiers);
    }

    public void Load(Momento momento) {
        if (momento is not SaveData save) {
            return;
        }

        this.Attributes.Clear();
        this.Modifiers.Clear();
        /*foreach (KeyValuePair<Enum, Attribute> attribute in save.Attributes) {
            this.Attributes.Add(attribute.Key, attribute.Value);
        }

        foreach (KeyValuePair<Enum, Vector4> modifier in save.Modifiers) {
            this.Modifiers.Add(modifier.Key, modifier.Value);
        }*/

        foreach (Enum attribute in this.Attributes.Keys) {
            this.Recompute(attribute);
        }
    }

    [Serializable]
    private sealed class SaveData : Momento {
        [field: SerializeField]
        private List<Attribute> SavedAttributes { get; set; }

        [field: SerializeField]
        private List<string> ModifiedAttributes { get; set; }

        [field: SerializeField]
        private List<Vector4> Modifiers { get; set; }

        public IReadOnlyList<Attribute> Attributes => this.SavedAttributes;
        // public IReadOnlyDictionary<Enum, Vector4> 

        public SaveData(
            Guid id, ReadOnlyDictionary<Enum, Attribute> attributes,
            ReadOnlyDictionary<Enum, Vector4> modifiers
        ) : base(id.ToString()) {
            // this.Attributes = [..attributes];
            // this.Modifiers = modifiers;
        }
    }
}
