namespace Project.Scripts.Util.Visitor;

public interface IVisitable<out T> where T : IVisitable<T> {
    void Accept(IVisitor<T> visitor);
}