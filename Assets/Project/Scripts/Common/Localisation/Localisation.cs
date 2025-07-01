using SaintsField;
using UnityEngine;

namespace Project.Scripts.Common.Localisation;

public abstract class Localisation<T> : ScriptableObject {
    [field: SerializeField, SaintsDictionary("Tag", "Localisation")]
    protected SaintsDictionary<string, string> Entries { get; private set; } = [];
    
    public abstract string Map(T key);
    
    public abstract void Refetch();

    public virtual string CreateDefault(T key) {
        return key?.ToString() ?? "null";
    }
}
