using System;
using DunGen;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Map;

public class GameMap : MonoBehaviour {
    public void Spawn(GameObject entity, Transform at) {
        Object.Instantiate(entity, at.position, at.rotation);
    }
}
