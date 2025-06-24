namespace Project.Scripts.Util.Builder;

public class FluentBuilder<T> {
    protected T Template { get; set; }

    protected FluentBuilder(T template) {
        this.Template = template;
    }

    public virtual T Build() {
        return this.Template;
    }
}
