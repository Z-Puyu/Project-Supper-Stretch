using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    internal interface IPresenter {
        internal string Name { get; }
        internal void Bind(UiPage page);
        internal void Bind(GameObject model, VisualElement view);
    }
    
    internal interface IPresenter<in T> : IPresenter {
        internal void Present(T data);
    }
}
