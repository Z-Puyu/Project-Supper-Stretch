using System;
using UnityEngine;

namespace CommonFrameworks.Async {
    public readonly record struct AsyncTask(Guid Id) {
        private AwaitableCompletionSource Source { get; } = new AwaitableCompletionSource();
        public Awaitable Awaitable => this.Source.Awaitable;

        public AsyncTask() : this(Guid.NewGuid()) {
            this.Source.Reset();
        }

        public static AsyncTask CompletedTask {
            get {
                AsyncTask task = new AsyncTask();
                task.TryComplete();
                return task;
            }
        }

        public bool TryComplete() {
            return this.Source.TrySetResult();
        }

        public bool TryInterrupt() {
            return this.Source.TrySetCanceled();
        }
        
        public static implicit operator Awaitable(AsyncTask task) => task.Awaitable;
    }

    public readonly record struct AsyncTask<T>(Guid Id) {
        private AwaitableCompletionSource<T> Source { get; } = new AwaitableCompletionSource<T>();
        public Awaitable<T> Awaitable => this.Source.Awaitable;

        public AsyncTask() : this(Guid.NewGuid()) {
            this.Source.Reset();
        }

        public static AsyncTask<T> FromResult(T result) {
            AsyncTask<T> task = new AsyncTask<T>();
            task.TryComplete(result);
            return task;
        }

        public bool TryComplete(T result) {
            return this.Source.TrySetResult(result);
        }

        public bool TryInterrupt() {
            return this.Source.TrySetCanceled();
        }
        
        public static implicit operator Awaitable<T>(AsyncTask<T> task) => task.Awaitable;
    }
}
