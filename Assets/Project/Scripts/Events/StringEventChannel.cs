using UnityEngine;

namespace Project.Scripts.Events;

[CreateAssetMenu(fileName = "String Event Channel", menuName = "Event System/String Event Channel")]
public class StringEventChannel : EventChannel<string>;
