using System;
using UnityEngine;

namespace CommonFrameworks.Processors {
    [Serializable]
    public abstract class Processor<T> : IProcessor<T> {
        private Processor<T>? Next { get; set; }

        [field: SerializeField]
        private ProcessorChainingPolicy ChainingPolicy { get; set; } = ProcessorChainingPolicy.AlwaysContinue;

        public T Process(T data) {
            bool isSuccessful = this.TryProcess(data, out T result);
            bool shouldContinue = this.ChainingPolicy == ProcessorChainingPolicy.AlwaysContinue ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnFailure && isSuccessful) ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnSuccess && !isSuccessful);
            return shouldContinue && this.Next is not null ? this.Next.Process(result) : result;
        }

        public Processor<T> Then(Processor<T> processor) {
            this.Next = processor;
            return processor;
        }
        
        protected abstract bool TryProcess(T data, out T result);
    }
}