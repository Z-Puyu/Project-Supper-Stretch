using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Util.BooleanLogic;

[Serializable]
public class And : ITestable {
    [field: SerializeReference] 
    private List<ITestable> Subconditions { get; set; } = [];

    public bool Holds() {
        return this.Subconditions.TrueForAll(condition => condition.Holds());
    }
}