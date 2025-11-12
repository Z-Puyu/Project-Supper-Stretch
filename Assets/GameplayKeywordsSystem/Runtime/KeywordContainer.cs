using CommonFrameworks.Trees;
using UnityEngine;

namespace GameplayKeywordsSystem.Runtime {
    [DisallowMultipleComponent]
    public sealed class KeywordContainer : TrieSetComponent<Keyword, char> {
        public KeywordContainer() : base(new TrieSet<Keyword, char>('.')) { }
    }
}
