using UnityEngine;
using UnityEngine.AI;

namespace Project.Scripts.Util.Components;

public static class NavMeshUtil {
    public static bool FoundRandomPoint(this Vector3 centre, float radius, out Vector3 point, int maxTries = 20) {
        NavMeshHit hit;
        point = centre + Random.insideUnitSphere * radius;
        while (!NavMesh.SamplePosition(point, out hit, 1, NavMesh.AllAreas) && maxTries != 0) {
            point = centre + Random.insideUnitSphere * radius;
            maxTries -= 1;
        }

        if (maxTries == 0) {
            point = Vector3.zero;
            return false;
        }
        
        point = hit.position;
        return true;
    }
}
