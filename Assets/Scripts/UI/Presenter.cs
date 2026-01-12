using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    [Serializable]
    internal abstract class Presenter<M, V, T> : IPresenter<T> where M : class where V : VisualElement {
        private UiPage? Owner { get; set; }
        [NotNull] [field: SerializeField] protected M? Model { get; set; }
        [NotNull] protected V? ViewRoot { get; set; }
        
        [field: SerializeField, Dropdown(nameof(this.UniqueElements), false, EUnique.Remove)] 
        protected string View { get; set; } = string.Empty;

        string IPresenter.Name => this.View.Split(' ')[0];
        
        protected DropdownList<string> UniqueElements => this.Owner
                ? this.Owner.FetchUniqueElements<V>()
                : new DropdownList<string>();

        void IPresenter.Bind(UiPage page) {
            this.Owner = page;
        }

        public virtual void Bind(GameObject model, VisualElement view) {
            VisualElementIdentifier identifier = VisualElementIdentifier.Parse(this.View);
            this.ViewRoot = identifier.Name.Split('/')
                                      .Aggregate(view, (current, part) => current.Q(part))
                                      .Q<V>(classes: identifier.Classes);
        }
        
        public abstract void Present(T data);
    }
}
