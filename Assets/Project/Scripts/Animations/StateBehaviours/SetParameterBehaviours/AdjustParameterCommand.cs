using System;
using UnityEngine;

namespace Project.Scripts.Animations.StateBehaviours.SetParameterBehaviours;

[Serializable]
public abstract class AdjustParameterCommand {
    protected Lazy<int> ParameterId { private set; get; }

    [field: SerializeField]
    protected string Parameter { get; private set; } = "";

    public AdjustParameterCommand() {
        this.ParameterId = new Lazy<int>(() => Animator.StringToHash(this.Parameter));
    }
    
    public abstract void Execute(Animator animator);
}
