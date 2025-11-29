using System;

namespace CommonFrameworks.Transformers {
    public delegate T Mapping<in S, out T>(S input);
    
    public readonly struct Pipeline<S, T> : ITransformer<S, T> {
        private ITransformer<S, T> Transformer { get; }

        private Pipeline(ITransformer<S, T> transformer) {
            this.Transformer = transformer;
        }

        public static Pipeline<S, T> StartFrom(ITransformer<S, T> transformer) {
            return new Pipeline<S, T>(transformer);
        }

        public static Pipeline<S, T> StartFrom(Func<S, T> transformer) {
            return new Pipeline<S, T>(new Transformer<S, T>(transformer));
        }

        public Pipeline<S, U> Then<U>(ITransformer<T, U> transformer) {
            return new Pipeline<S, U>(new ComposedTransformer<S, T, U>(this.Transformer, transformer));
        }

        public Pipeline<S, U> Then<U>(Func<T, U> transformer) {
            return this.Then(new Transformer<T, U>(transformer));
        }

        public Mapping<S, T> Compile() {
            return this.Transform;
        }
        
        public T Transform(S data) {
            return this.Transformer.Transform(data);
        }
    }
}
