using System;
using CommonFrameworks.Extensions;
using GameplayAbilitiesSystem.Runtime.Attributes;
using SaintsField;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI {
    [Serializable]
    internal sealed class StatBarPresenter : Presenter<AttributeSet, ProgressBar, (float current, float max)> {
        [field: SerializeField, TreeDropdown(nameof(this.AllAttributes))]
        private string TrackedAttribute { get; set; } = string.Empty;
        
        [field: SerializeField, EndText("px"), Tooltip("Set to 0 to disable stat bar scaling")] 
        private float UnitLength { get; set; } = 0;
        
        private AdvancedDropdownList<string> AllAttributes => AttributeUtils.GetLeafAttributes();
        
        public override void Bind(GameObject model, VisualElement view) {
            base.Bind(model, view);
            if (!this.Model) {
                this.Model = model.GetClosestComponentInChildren<AttributeSet>();
            }
            
            this.ViewRoot.lowValue = (float)this.Model!.QueryMin(this.TrackedAttribute);
            float max = (float)this.Model.QueryMax(this.TrackedAttribute);
            float current = (float)this.Model.Query(this.TrackedAttribute);
            this.Present((current, max));
            this.Model.OnAnyAttributeUpdated += this.HandleAttributeUpdate;
        }

        private void HandleAttributeUpdate(AttributeKey key, AttributeChange change) {
            if (key != this.TrackedAttribute) {
                return;
            }

            float max = (float)this.Model.QueryMax(this.TrackedAttribute);
            float current = (float)this.Model.Query(this.TrackedAttribute);
            this.Present((current, max));
        }

        public override void Present((float current, float max) data) {
            this.ViewRoot.highValue = data.max;
            this.ViewRoot.value = data.current;
            if (this.UnitLength > 0) {
                this.ViewRoot.style.width = this.UnitLength * data.max;
            }
        }
    }
}
