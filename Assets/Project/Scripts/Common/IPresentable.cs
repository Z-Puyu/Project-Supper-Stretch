namespace Project.Scripts.Common;

public interface IPresentable {
    public abstract string FormatAsText();
}

public interface IPresentable<in T> where T : class {
    public abstract string FormatAsText(T reference);   
}
