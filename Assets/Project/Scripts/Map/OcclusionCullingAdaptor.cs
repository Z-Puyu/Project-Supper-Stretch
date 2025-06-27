using System.Collections;
using DunGen;
using DunGen.Adapters;
using UnityEditor;
using UnityEngine;

namespace Project.Scripts.Map;

public class OcclusionCullingAdaptor : BaseAdapter {
    private static IEnumerator BakingCoroutine() {
        // Ensure all objects are properly set up
        yield return new WaitForEndOfFrame();
        
        // Bake occlusion culling data
        StaticOcclusionCulling.Compute();
        
        while (StaticOcclusionCulling.isRunning) {
            yield return null;
        }
        
        Debug.Log("Occlusion baking complete!");
    }

    protected override void Run(DungeonGenerator generator) {
        this.StartCoroutine(OcclusionCullingAdaptor.BakingCoroutine());
    }
}
