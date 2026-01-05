using System;
using UnityEngine;

namespace CommonFrameworks.Extensions {
    public static class AwaitableExtensions {
        public static async Awaitable WaitUntilAsync(Func<bool> predicate) {
            while (!predicate()) {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}
