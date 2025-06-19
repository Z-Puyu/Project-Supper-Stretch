using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[CreateAssetMenu(fileName = "Attribute Relation", menuName = "Attribute System/Attribute Relation")]
public class AttributeRelation : ScriptableObject {
    [field: SerializeField, Table] private List<AttributeRelationDefinition> Relations { get; set; } = [];

    public bool ContainsPositiveRelation(AttributeKey from, out AttributeKey to) {
        foreach (AttributeRelationDefinition def in this.Relations.Where(def => def.Key == from)) {
            to = def.PositiveRelative;
            return true;
        }
        
        to = default;
        return false;
    }
    
    public bool ContainsNegativeRelation(AttributeKey from, out AttributeKey to) {
        foreach (AttributeRelationDefinition def in this.Relations.Where(def => def.Key == from)) {
            to = def.NegativeRelative;
            return true;
        }   
        
        to = default;
        return false;       
    }
    
    public bool ContainsRelation(AttributeKey from, out AttributeKey to) {
        return this.ContainsPositiveRelation(from, out to) || this.ContainsNegativeRelation(from, out to);
    }
}
