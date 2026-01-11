using System;
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
        private VisualElement Root => this.Document.rootVisualElement;

        [field: SerializeReference, ReferencePicker, FieldLabelText(nameof(this.LabelPresenter), true)]
        private List<IPresenter> Presenters { get; set; } = new List<IPresenter>();

        private void Awake() {
            this.Document = this.GetComponent<UIDocument>();
        }
        
        private string LabelPresenter(IPresenter presenter, int index) {
            return $"[{index}] {presenter.Name}";
        }
        
        internal DropdownList<string> FetchUniqueElements() {
            VisualElement root = this.Document ? this.Root : this.GetComponent<UIDocument>().rootVisualElement;
            IEnumerable<(string, string)> identifiers =
                    root.FetchNamedChildren<VisualElement>().OrderBy(element => element.name).Select(selector);
            
            return new DropdownList<string>(identifiers);

            static (string, string) selector(VisualElement element) {
                VisualElementIdentifier id = new VisualElementIdentifier(element);
                return (id.ToString(), id.ToString());
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
