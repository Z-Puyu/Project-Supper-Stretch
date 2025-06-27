using System.Collections.Generic;
using System.Linq;

namespace Project.Scripts.AttributeSystem.Modifiers;

public static class ModifierUtil {
    public static IEnumerable<Modifier> GetModifiers(this IEnumerable<ModifierData> effects) {
        Dictionary<ModifierKey, Modifier> instant = [];
        Dictionary<ModifierKey, Dictionary<int, Modifier>> timed = [];
        foreach (ModifierData data in effects) {
            Modifier modifier = Modifier.From(data);
            if (data.Duration != 0) {
                if (!timed.TryGetValue(data.Key, out Dictionary<int, Modifier>? timedModifiers)) {
                    timed.Add(data.Key, new Dictionary<int, Modifier> { { data.Duration, modifier } });
                } else if (!timedModifiers.TryAdd(data.Duration, modifier)) {
                    timedModifiers[data.Duration] += modifier;
                }
            } else if (!instant.TryAdd(data.Key, modifier)) {
                instant[data.Key] += modifier;
            }
        }
        
        return instant.Values.Concat(timed.Values.SelectMany(modifiers => modifiers.Values));
    }
}
