using UnityEngine;

namespace Project.Scripts.InteractionSystem;

public class ItemPickUp : InteractableObject {
    protected override void OnInteraction() {
        Debug.Log("Picking up item");
    }
}
