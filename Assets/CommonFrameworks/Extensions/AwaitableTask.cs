using System;
using UnityEngine;

namespace CommonFrameworks.Extensions {
    public static class AwaitableTask {
        private static readonly AwaitableCompletionSource Completion = new AwaitableCompletionSource();

        public static Awaitable CompletedTask {
            get {
                AwaitableTask.Completion.SetResult();
                Awaitable? awaitable = AwaitableTask.Completion.Awaitable;
                AwaitableTask.Completion.Reset();
                return awaitable;
            }
        }

        public static async Awaitable WaitUntilAsync(Func<bool> predicate) {
            while (!predicate.Invoke()) {
                await Awaitable.NextFrameAsync();
            }
        }

        public static async Awaitable WaitUntilAsync<T>(T args, Func<T, bool> predicate) {
            while (!predicate.Invoke(args)) {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
