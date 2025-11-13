namespace CommonFrameworks.CommonUtilities.CommonInterfaces {
    public interface ICommand<in T> {
        public void Execute(T target);
        public void Undo(T target);
    }

    public interface ICommand {
        public void Execute();
        public void Undo();
    }
}
