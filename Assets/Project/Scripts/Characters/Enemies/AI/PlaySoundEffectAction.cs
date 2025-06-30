using Project.Scripts.Characters.Enemies.AI;
using System;
using Project.Scripts.Characters;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Sound Effect", story: "[Agent] plays a random sound effect given [CurrentEnemyStatus]", category: "Action/Enemy AI", id: "e750778311400dd005fb558a8e594fad")]
public partial class PlaySoundEffectAction : Action {
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<EnemyStatus> CurrentEnemyStatus;
    private CharacterAudio? Audio { get; set; }

    protected override Status OnStart() {
        this.Audio = this.Agent.Value.GetComponent<CharacterAudio>();
        if (!this.Audio) {
            return Status.Failure;
        }

        switch (this.CurrentEnemyStatus.Value) {
            case EnemyStatus.Idle or EnemyStatus.Pursue or EnemyStatus.Patrol:
                this.Audio.Play(CharacterAudio.Sound.Idle);
                break;
            case EnemyStatus.Engaged:
                this.Audio.Play(CharacterAudio.Sound.BattleCry);
                break;
        }
        
        return Status.Success;
    }
}

