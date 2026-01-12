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

        private void Awake() {
            this.Document = this.GetComponent<UIDocument>();
        }

        private string LabelPresenter(IPresenter presenter, int index) {
            return $"[{index}] {presenter.Name}";
        }
        
        internal DropdownList<string> FetchUniqueElements<V>() where V : VisualElement {
            VisualElement root = this.Document ? this.Root : this.GetComponent<UIDocument>().rootVisualElement;
            IEnumerable<(string, string)> identifiers = root.FetchNamedChildren<V>()
                                                            .OrderBy(element => element.name)
                                                            .Select(selector);
            return new DropdownList<string>(identifiers);

            (string, string) selector(V element) {
                VisualElementIdentifier id = new VisualElementIdentifier(element, root);
                return string.IsNullOrWhiteSpace(id.Name) ? ("root", id.ToString()) : (id.ToString(), id.ToString());
            }
        }

        public abstract void Open();
        public abstract void Close();

        private void OnValidate() {
            foreach (IPresenter presenter in this.Presenters) {
                presenter.Bind(this);
            }
        }
    }
}
