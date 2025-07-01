using System.Collections;
using DunGen;
using DunGen.Adapters;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Map;

public class OcclusionCullingAdaptor : BaseAdapter {
    protected override void Run(DungeonGenerator generator) {
        if (!Camera.main || !Camera.main.TryGetComponent(out OcclusionCulling culling)) {
            return;
        }
        
        foreach (Occludee occludee in generator.CurrentDungeon.GetComponentsInChildren<Occludee>()) {
            culling.Register(occludee);
        }
    }
}
