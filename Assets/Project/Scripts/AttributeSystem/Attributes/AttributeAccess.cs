using System;
using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

public class AttributeAccess : MonoBehaviour, IAttributeReader {
    [NotNull]
    [field: SerializeField]
    [field: InfoBox("If unassigned, the component will fetch the closest one in its parents.")]
    private AttributeSet? Source { get; set; }
    
    public AdvancedDropdownList<string> AllAccessibleAttributes => !this.Source 
            ? this.GetComponentInParent<AttributeSet>().AllAccessibleAttributes
            : this.Source.AllAccessibleAttributes;

    private void Awake() {
        if (this.Source) {
            return;
        }

        this.Source = this.GetComponentInParent<AttributeSet>();
        if (!this.Source) {
            throw new InvalidOperationException("No attribute set found in parents.");
        }
    }

    public Attribute Read(string attribute) {
        return this.Source.Read(attribute);
    }
    
    public int ReadCurrent(string attribute) {
        return this.Source.ReadCurrent(attribute);
    }
    
    public int ReadBase(string attribute) {
        return this.Source.ReadBase(attribute);
    }
    
    public int ReadMax(string attribute) {
        Attribute value = this.Read(attribute);
        if (value.Cap == string.Empty) {
            return value.HardLimit >= 0 ? value.HardLimit : int.MaxValue;
        }
        
        return this.ReadCurrent(value.Cap);
    }
}
