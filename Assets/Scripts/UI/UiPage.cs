using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SaintsField;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    [DisallowMultipleComponent, RequireComponent(typeof(UIDocument))]
    public abstract class UiPage : MonoBehaviour, IPage {
        [NotNull] private UIDocument? Document { get; set; }
        private protected VisualElement Root => this.Document.rootVisualElement;

        [field: SerializeReference, ReferencePicker, FieldLabelText(nameof(this.LabelPresenter), true)]
        private protected List<IPresenter> Presenters { get; private set; } = new List<IPresenter>();
        
        private IEnumerable<string> ui = Enumerable.Empty<string>();

        protected virtual void Awake() {
            this.Document = this.GetComponent<UIDocument>();
        }

        private string LabelPresenter(IPresenter? presenter, int index) {
            return $"[{index}] {presenter?.Name}";
        }
        
        internal IEnumerable<string> FetchUniqueElements<V>() where V : VisualElement {
            VisualElement root = this.GetComponent<UIDocument>().visualTreeAsset.CloneTree();
            return root.FetchNamedChildren<V>()
                       .Select(element => new VisualElementIdentifier(element, root))
                       .Select(id => string.IsNullOrWhiteSpace(id.Name) ? "root" : id.ToString());
        }

        public virtual void Open() {
            this.gameObject.SetActive(true);
        }

        public virtual void Close() {
            this.gameObject.SetActive(false);
        }

        private void OnValidate() {
            foreach (IPresenter presenter in this.Presenters) {
                presenter.Bind(this);
            }
        }
    }
}
