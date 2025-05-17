using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.GameplayEffects;

[DisallowMultipleComponent]
[RequireComponent(typeof(AttributeSet), typeof(ModifierManager))]
public sealed class GameplayEffectManager : MonoBehaviour {
    [NotNull]
    private AttributeSet? AttributeSet { get; set; }
    
    [NotNull]
    private ModifierManager? ModifierManager { get; set; }

    private void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
        this.ModifierManager = this.GetComponent<ModifierManager>();
    }
    
    /// <summary>
    /// Accepts and applies a gameplay effect on the owning game object.
    /// </summary>
    /// <param name="effect">The gameplay effect to apply.</param>
    /// <param name="params">The configuration parameters defining how the effect should be applied.</param>
    public void Consume(GameplayEffect effect, GameplayEffectInvocationParameter @params) {
        effect.Invoke(@params.Instigator, this.AttributeSet, @params.Magnitudes, @params.Chance)
              .ForEach(this.ModifierManager.Accept);
    }
}
