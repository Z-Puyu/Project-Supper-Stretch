using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes;

public class AttributeAccess : MonoBehaviour, IAttributeReader {
    [NotNull]
    [field: SerializeField]
    [field: InfoBox("If unassigned, the component will fetch the closest one in its parents.")]
    private AttributeSet? Source { get; set; }

    private void Awake() {
        if (this.Source) {
            return;
        }

        this.Source = this.GetComponentInParent<AttributeSet>();
        if (!this.Source) {
            throw new InvalidOperationException("No attribute set found in parents.");
        }
    }

    public Attribute Read(Enum attribute) {
        return this.Source[attribute];
    }
    
    public int ReadCurrent(Enum attribute) {
        return this.Source[attribute].CurrentValue;
    }
    
    public int ReadBase(Enum attribute) {
        return this.Source[attribute].BaseValue;
    }
    
    public int ReadMax(Enum attribute) {
        Attribute value = this.Read(attribute);
        if (value.Cap is null) {
            return value.HardLimit >= 0 ? value.HardLimit : int.MaxValue;
        }
        
        return this.ReadCurrent(value.Cap);
    }
}
