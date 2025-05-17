using UnityEngine;

namespace Project.Scripts.Events;

[CreateAssetMenu(fileName = "GameObject Event Channel", menuName = "Event System/GameObject Event Channel")]
public class GameObjectEventChannel : EventChannel<GameObject?>;
