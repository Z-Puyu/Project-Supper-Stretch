namespace CommonFrameworks.Processors {
    public enum ProcessorChainingPolicy {
        BreakOnlyOnSuccess,
        BreakOnlyOnFailure,
        AlwaysBreak,
        AlwaysContinue,
    }
}
