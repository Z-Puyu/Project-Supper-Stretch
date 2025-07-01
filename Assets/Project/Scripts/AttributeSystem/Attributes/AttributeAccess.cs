using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

public class AttributeAccess : MonoBehaviour, IAttributeReader {
    [NotNull]
    [field: SerializeField]
    [field: InfoBox("If unassigned, the component will fetch the closest one in the hierarchy of root object.")]
    private AttributeSet? Source { get; set; }
    
    public AdvancedDropdownList<string> AllAccessibleAttributes => !this.Source 
            ? this.transform.root.GetComponentInChildren<AttributeSet>().AllAccessibleAttributes
            : this.Source.AllAccessibleAttributes;

    private void Awake() {
        if (this.Source) {
            return;
        }

        this.Source = this.transform.root.GetComponentInChildren<AttributeSet>();
        if (!this.Source) {
            throw new InvalidOperationException("No attribute set found.");
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
        return this.Source.ReadMax(attribute);
    }
}
