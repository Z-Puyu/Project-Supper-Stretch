using System;
using UnityEngine;

namespace CommonFrameworks.Processors {
    [Serializable]
    public abstract class Processor<T> : IProcessor<T> {
        private Processor<T>? Next { get; set; }

        [field: SerializeField]
        private ProcessorChainingPolicy ChainingPolicy { get; set; } = ProcessorChainingPolicy.AlwaysContinue;

        public void Process(ref T data) {
            bool isSuccessful = this.TryProcess(ref data);
            bool shouldContinue = this.ChainingPolicy == ProcessorChainingPolicy.AlwaysContinue ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnFailure && isSuccessful) ||
                                  (this.ChainingPolicy == ProcessorChainingPolicy.BreakOnlyOnSuccess && !isSuccessful);
            if (shouldContinue) {
                this.Next?.Process(ref data);
            } 
        }

        public Processor<T> Then(Processor<T> processor) {
            this.Next = processor;
            return processor;
        }
        
        /// <summary>
        /// Attempts to process the given data.
        /// </summary>
        /// <param name="data">The input data to process.</param>
        /// <returns>True if the processing was successful, false otherwise.</returns>
        protected abstract bool TryProcess(ref T data);
    }
}