using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using GameplayKeywordsSystem.Runtime;

namespace GameplayAbilitiesSystem.Runtime.Effects {
    internal abstract class ContinuousEffect {
        internal EffectDescriptor Descriptor { get; set; }
        [NotNull] private protected IEffectReceiverFacade? Target { get; private set; }
        private ICollection<Keyword> RemovedKeywords { get; set; } = new List<Keyword>();
        private ICollection<Keyword> AddedKeywords { get; set; } = new List<Keyword>();
        private CancellationTokenSource? CancellationTokenSource { get; set; }

        private protected CancellationToken CancellationToken =>
                this.CancellationTokenSource?.Token ?? CancellationToken.None;
        
        private protected bool IsAlive => !this.CancellationToken.IsCancellationRequested;

        private protected void Apply(
            IEffectReceiverFacade target, IEnumerable<string> addedKeywords, IEnumerable<string> removedKeywords
        ) {
            this.Target = target;
            foreach (Keyword keyword in removedKeywords) {
                this.RemovedKeywords.Add(keyword);
                target.ReceiverKeywordContainer.Remove(keyword);
            }

            foreach (Keyword keyword in addedKeywords) {
                this.AddedKeywords.Add(keyword);
                target.ReceiverKeywordContainer.Add(keyword);
            }
        }

        internal virtual void Stop() {
            foreach (Keyword keyword in this.RemovedKeywords) {
                this.Target.ReceiverKeywordContainer.Add(keyword);
            }

            foreach (Keyword keyword in this.AddedKeywords) {
                this.Target.ReceiverKeywordContainer.Remove(keyword);
            }
            
            this.CancellationTokenSource?.Cancel();
        }

        internal virtual void Reset() {
            this.Target = null;
            this.Descriptor = EffectDescriptor.Empty;
            this.AddedKeywords.Clear();
            this.RemovedKeywords.Clear();
            if (this.CancellationTokenSource is null) {
                this.CancellationTokenSource = new CancellationTokenSource();
            } else if (this.CancellationTokenSource.IsCancellationRequested) {
                this.CancellationTokenSource.Dispose();
                this.CancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}