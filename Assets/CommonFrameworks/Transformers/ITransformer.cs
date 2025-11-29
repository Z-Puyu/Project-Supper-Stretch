namespace CommonFrameworks.Transformers {
    public interface ITransformer<in S, out T> {
        public T Transform(S data);        
    }
}
