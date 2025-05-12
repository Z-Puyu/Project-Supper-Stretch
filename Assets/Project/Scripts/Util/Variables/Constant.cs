using UnityEngine;

namespace Project.Scripts.Util.Variables;

public class Constant<T> : IReadable<T> {
    [field: SerializeField] 
    private T? Value { get; set; }
    
    public T? Read() {
        return this.Value;
    }
    
    public override string ToString() {
        return $"{typeof(T)} constant: {this.Value}";
    }
}
