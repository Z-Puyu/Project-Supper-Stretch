using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[CreateAssetMenu(fileName = "AttributeConversion", menuName = "Attribute System/Definitions/Attribute Conversion", order = 0)]
public class AttributeConversion : ScriptableObject {
    [field: SerializeField, Table] private List<AttributeConversionRule> Rules { get; set; } = [];

    public float Convert(float value, string from, string to) {
        AttributeConversionRule? rule = this.Rules.FirstOrDefault(rule => rule.From == from && rule.To == to);
        if (rule is null) {
            return 0;
        }
        
        return value * rule.ConversionRate;
    }

    public bool TryConvert(float value, string from, out IReadOnlyDictionary<string, float> results) {
        Dictionary<string, float> converted = [];
        foreach (AttributeConversionRule rule in this.Rules.Where(rule => rule.From == from)) {
            converted.Add(rule.To, value * rule.ConversionRate);
        }
        
        results = converted;
        return results.Any();       
    }
    
    public bool TryConvert(float value, string from, out (string key, float value) result) {
        AttributeConversionRule? rule = this.Rules.FirstOrDefault(rule => rule.From == from);
        if (rule is null) {
            result = (string.Empty, 0);
            return false;
        }
        
        result = (rule.To, value * rule.ConversionRate);
        return true;
    }

    public bool TryConvertTo(string to, float value, out IReadOnlyDictionary<string, float> results) {
        Dictionary<string, float> converted = [];
        foreach (AttributeConversionRule rule in this.Rules.Where(rule => rule.To == to)) {
            converted.Add(rule.From, value / rule.ConversionRate);
        }
        
        results = converted;
        return results.Any();       
    }
    
    public bool TryConvertTo(string to, float value, out float result) {
        AttributeConversionRule? rule = this.Rules.FirstOrDefault(rule => rule.To == to);
        if (rule is null) {
            result = 0;
            return false;
        }
        
        result = value / rule.ConversionRate;   
        return true;       
    }
}
