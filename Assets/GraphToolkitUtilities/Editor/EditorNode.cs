using GraphToolkitUtilities.Runtime;
using Unity.GraphToolkit.Editor;

namespace GraphToolkitUtilities.Editor {
    public abstract class EditorNode<R> : Node where R : RuntimeNode {
        internal abstract R MakeRuntimeNode();
    }
}
