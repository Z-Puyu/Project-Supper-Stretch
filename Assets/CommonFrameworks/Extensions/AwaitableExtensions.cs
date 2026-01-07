using System;
using UnityEngine;

namespace CommonFrameworks.Extensions {
    public static class AwaitableExtensions {
        private static readonly AwaitableCompletionSource Completion = new AwaitableCompletionSource();
        
        public static Awaitable CompletedTask {
            get {
                AwaitableExtensions.Completion.SetResult();
                Awaitable? awaitable = AwaitableExtensions.Completion.Awaitable;
                AwaitableExtensions.Completion.Reset();
                return awaitable;
            }
        }
        
        public static async Awaitable WaitUntilAsync(Func<bool> predicate) {
            while (!predicate()) {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
