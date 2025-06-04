using System;
using UnityEngine;

namespace Project.Scripts.Util.BooleanLogic;

[Serializable]
public class Not : ITestable {
    [field: SerializeReference] 
    private ITestable? Subcondition { get; set; }
        
    public bool Holds() {
        return !(this.Subcondition?.Holds() ?? true);
    }
}