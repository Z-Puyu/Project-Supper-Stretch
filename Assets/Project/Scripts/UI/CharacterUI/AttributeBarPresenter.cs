using System;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.UI.Control;
using UnityEngine;

namespace Project.Scripts.UI.CharacterUI;

public abstract class AttributeBarPresenter<E> : ProgressBarPresenter<IAttributeReader> where E : Enum {
    [field: SerializeField]
    protected E Attribute { get; private set; }

    protected AttributeBarPresenter(E attribute) {
        this.Attribute = attribute;
    }
    
    public override void Present() {
        if (this.Model == null) {
            return;
        }
        
        this.View.Display((this.Model.ReadCurrent(this.Attribute), this.Model.ReadMax(this.Attribute)));
    }
}
