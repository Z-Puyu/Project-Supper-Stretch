namespace Project.Scripts.Util.Visitor;

public interface IVisitor<in T> where T : IVisitable<T> {
    void Visit(T visitable);
}
