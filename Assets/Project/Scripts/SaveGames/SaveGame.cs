using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Util.DataPersistence;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.SaveGames;

public class SaveGame {
    public string SessionId { get; set; }
    private List<Momento> SavedObjects { get; init; } = [];

    public void Register(Momento data) {
        this.SavedObjects.Add(data);
    }

    public void Load() {
        Dictionary<Guid, IPersistent> objects = 
                Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                      .OfType<IPersistent>()
                      .ToDictionary(obj => obj.Id, obj => obj);
        foreach (Momento data in this.SavedObjects) {
            Guid guid = Guid.Parse(data.Id);
            if (objects.TryGetValue(guid, out IPersistent obj)) {
                obj.Load(data);
            } else {
                Debug.LogWarning($"Failed loading to {obj}.");
            }
        }
    }
}
