namespace CommonFrameworks.CommonUtilities.CommonInterfaces {
    public interface ICommand<in T> {
        public void Execute(T target);
        public void Undo(T target);
    }
}
