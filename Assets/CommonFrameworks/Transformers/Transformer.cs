using System;

namespace CommonFrameworks.Transformers;

internal readonly struct Transformer<S, T> : ITransformer<S, T> {
    private Func<S, T> Delegate { get; }
        
    internal Transformer(Func<S, T> @delegate) {
        this.Delegate = @delegate;
    }
        
    public T Transform(S data) {
        return this.Delegate(data);
    }       
}