using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

#if UNITY_EDITOR
namespace Project.Scripts.Characters.Enemies.AI {
    [CreateAssetMenu(menuName = "Behavior/Event Channels/HostileTargetDetected")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "EngagedInCombat", message: "[Agent] is in combat with [Target] ? [Boolean]",
        category: "Events", id: "a1a98de4707305ec411ea582438878bb")]
    public sealed partial class EngagedInCombat : EventChannel<GameObject, GameObject, bool>;
}

