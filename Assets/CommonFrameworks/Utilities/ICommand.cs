namespace CommonFrameworks.CommonUtilities {
    public interface ICommand {
        public void Execute();
        public void Undo();
    }
}
