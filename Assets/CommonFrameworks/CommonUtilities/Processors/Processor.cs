using System;
using UnityEngine;

namespace CommonFrameworks.CommonUtilities.Processors {
    [Serializable]
    public abstract class Processor<T> : IProcessor<T> {
        private Processor<T> Next { get; set; }

        [field: SerializeField]
        private ProcessorChainingPolicy ChainingPolicy { get; set; } = ProcessorChainingPolicy.AlwaysContinue;

        public T Process(T data) {
            T processedData = this.TryProcess(data, out bool isSuccessful);
            bool shouldContinue = this.ChainingPolicy == ProcessorChainingPolicy.AlwaysContinue ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnFailure && isSuccessful) ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnSuccess && !isSuccessful);
            return shouldContinue && this.Next is not null ? this.Next.Process(processedData) : processedData;
        }

        public Processor<T> Then(Processor<T> processor) {
            this.Next = processor;
            return processor;
        }
        
        protected abstract T TryProcess(T data, out bool isSuccessful);
    }
}
