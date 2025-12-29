namespace CommonFrameworks.Transformers {
    internal readonly struct ComposedTransformer<A, B, C> : ITransformer<A, C> {
        private ITransformer<A, B> First { get; }
        private ITransformer<B, C> Second { get; }

        internal ComposedTransformer(ITransformer<A, B> first, ITransformer<B, C> second) {
            this.First = first;
            this.Second = second;
        }
        
        public C Transform(A data) {
            return this.Second.Transform(this.First.Transform(data));
        }
    }
}