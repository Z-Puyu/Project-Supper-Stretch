using System.Collections;
using System.Collections.Generic;
using CommonFrameworks.Utilities;
using SaintsField;
using UnityEngine;

namespace GameplayAbilitiesSystem.Runtime.Attributes;

[CreateAssetMenu(fileName = "New Attribute Table", menuName = "Gameplay Abilities/Attribute Table")]
public sealed class AttributeTable : ScriptableObject, IEnumerable<KeyValuePair<AttributeType, double>>, IComponentInitialiser<AttributeSet> {
    [field: SerializeField, SaintsDictionary]
    private SaintsDictionary<AttributeType, double> BaseValues { get; set; } =
        new SaintsDictionary<AttributeType, double>(); 
        
    public IEnumerator<KeyValuePair<AttributeType, double>> GetEnumerator() {
        return this.BaseValues.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
        
    public void Initialise(AttributeSet component) {
        if (!component) {
            Debug.LogError("Cannot initialise null AttributeSet", this);
            return;
        }
            
        component.Clear();
        foreach (KeyValuePair<AttributeType, double> entry in this) {
            component.Initialise(entry.Key, entry.Value);
        }
    }
}