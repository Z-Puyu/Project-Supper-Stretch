using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnKnockedBack")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnKnockedBack", message: "[Agent] is knocked back", category: "Events", id: "ff74b1deace214ff94238f399e757071")]
public sealed partial class OnKnockedBack : EventChannel<GameObject> { }

