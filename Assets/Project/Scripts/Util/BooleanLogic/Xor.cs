using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.Util.BooleanLogic;

[Serializable]
public class Xor : ITestable {
    [field: SerializeReference, SubclassSelector] 
    private List<ITestable> Subconditions { get; set; } = [];

    public bool Holds() {
        return this.Subconditions.Count(condition => condition.Holds()) == 1;
    }
}
