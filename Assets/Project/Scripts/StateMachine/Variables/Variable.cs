using System;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.StateMachine.Variables;

[Serializable]
public abstract record class Variable {
    [field: SerializeField, ValidateInput(nameof(this.IsUndefined))]
    public string Name { get; set; } = string.Empty;
    
    private bool IsUndefined => string.IsNullOrEmpty(this.Name);

    public override string ToString() {
        return this.IsUndefined ? "Undefined" : this.Name;
    }
}

[Serializable]
public abstract record class Variable<T> : Variable {
    [field: SerializeField]
    public T? Value { get; set; }
    
    public override string ToString() {
        return $"{typeof(T)} {base.ToString()}: {this.Value}";
    }
}

[Serializable]
public record class Integer : Variable<int>, IComparable {
    public int CompareTo(object obj) {
        return this.Value.CompareTo(obj);
    }
}

[Serializable]
public record class Float : Variable<float>, IComparable {
    public int CompareTo(object obj) {
        return this.Value.CompareTo(obj);
    }
}

[Serializable]
public record class Flag : Variable<bool>;

[Serializable]
public record class String : Variable<string>;

[Serializable]
public record class Vector2D : Variable<Vector2>;

[Serializable]
public record class LatticeVector2D : Variable<Vector2Int>;

[Serializable]
public record class Vector3D : Variable<Vector3>;

[Serializable]
public record class LatticeVector3D : Variable<Vector3Int>;

[Serializable]
public record class Vector4D : Variable<Vector4>;

[Serializable]
public record class GameObject : Variable<GameObject>;