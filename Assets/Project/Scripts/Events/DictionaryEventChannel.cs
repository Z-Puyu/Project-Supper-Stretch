using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Project.Scripts.Events;

[CreateAssetMenu(fileName = "Dictionary Event Channel", menuName = "Event System/Dictionary Event Channel")]
public class DictionaryEventChannel : EventChannel<SerializedDictionary<string, object>>;
