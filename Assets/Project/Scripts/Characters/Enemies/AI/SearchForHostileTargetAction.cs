using System;
using Project.Scripts.Interaction.ObjectDetection;
using Project.Scripts.Util.Components;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace Project.Scripts.Characters.Enemies.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SearchForHostileTarget", 
    story: "[Agent] searches for a hostile [Target] with [Sensor]", 
    category: "Action", 
    id: "d49fee93c09e8ffb504f1592345580ff")]
public partial class SearchForHostileTargetAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject?> Target;
    [SerializeReference] public BlackboardVariable<Sensor> Sensor;
    private CharacterAudio? Audio { get; set; }
    
    private void SpotHostileTarget(Collider target) {
        this.Target.Value = target.gameObject;
        this.Audio.IfPresent(component => component.Play(CharacterAudio.Sound.BattleCry));
        Debug.Log($"{this.Sensor.Value.transform.root.name} spotted hostile target {this.Target.Value}!");
    }

    protected override Status OnStart() {
        this.Audio = this.Agent.Value.GetComponent<CharacterAudio>();
        this.Sensor.Value.OnDetected += this.SpotHostileTarget;
        this.Sensor.Value.OnTargetLost += this.ResetTarget;
        return Status.Running;
    }

    private void ResetTarget(Collider target) {
        if (this.Target.Value == target.gameObject) {
            this.Target.Value = null;
        }
    }

    protected override Status OnUpdate() {
        return this.Target.Value ? Status.Success : Status.Running;
    }
    
    
}