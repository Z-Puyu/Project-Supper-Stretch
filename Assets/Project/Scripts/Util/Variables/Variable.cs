using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Util.Variables;

public abstract class Variable<T> : ScriptableObject, IEquatable<T>, IReadable<T> {
    [field: SerializeField] 
    public T? InitialValue { get; private set; }

    [SerializeField]
    private T? value;
    
    public T? CurrentValue {
        get => this.value;
        set {
            if (this.Equals(value)) {
                return;
            }

            T? old = this.value;
            this.value = value;
            this.OnValueChanged.Invoke(old, this.value);
        }
    }

    public event UnityAction<T?, T?> OnValueChanged = delegate { };
    
    public T? Read() {
        return this.CurrentValue;
    }

    public virtual bool Equals(T? other) {
        return this.CurrentValue?.Equals(other) ?? other == null;
    }
    
    public static implicit operator T?(Variable<T> variable) => variable.value;
}
