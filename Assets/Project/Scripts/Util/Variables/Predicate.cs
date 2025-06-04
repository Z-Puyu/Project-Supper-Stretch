using System;
using Project.Scripts.Util.BooleanLogic;
using UnityEngine;

namespace Project.Scripts.Util.Variables;

[Serializable]
public abstract class Predicate<T> : ITestable {
    [field: SerializeReference] 
    public IReadable<T>? Subject { get; set; }
    [field: SerializeReference] 
    public IReadable<T>? Object { get; set; }

    public abstract bool Holds();
}
