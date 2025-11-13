namespace CommonFrameworks.CommonUtilities.CommonInterfaces {
    public interface IPredicate {
        public bool Holds();
    }
    
    public interface IPredicate<in T> {
        public bool Holds(T args);
    }
}
