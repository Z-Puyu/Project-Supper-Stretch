using System.Threading;

namespace GameplayAbilitiesSystem.Runtime.Abilities {
    internal readonly record struct AbilityActivation(
        CancellationTokenSource Interrupter
    ) {
        internal void Interrupt() {
            this.Interrupter.Cancel();
            this.Interrupter.Dispose();
        }
    }
}
