using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    internal sealed class Hud : UiPage {
        [NotNull] 
        [field: SerializeField, Required] 
        public GameObject? Owner { get; private set; }

        private void Start() {
            foreach (IPresenter presenter in this.Presenters) {
                presenter.Bind(this.Owner, this.Root);
            }
        }

        public override void Open() { }

        public override void Close() { }
    }
}
