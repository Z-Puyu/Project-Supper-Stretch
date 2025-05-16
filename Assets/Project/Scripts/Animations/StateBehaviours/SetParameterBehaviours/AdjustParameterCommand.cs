using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public abstract class AdjustParameterCommand {
    protected int? ParameterId { private set; get; } = null;

    [field: SerializeField]
    protected string Parameter { get; private set; } = "";

    public virtual void Execute(Animator animator) {
        this.ParameterId ??= Animator.StringToHash(this.Parameter);
    }
}
