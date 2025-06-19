namespace Project.Scripts.Common.UI;

public record class UIData<T>(T Value) : IPresentable {
    public override string ToString() => $"{this.Value}";
    
    public static implicit operator UIData<T>(T value) => new UIData<T>(value);
    public static implicit operator T(UIData<T> data) => data.Value;
}
