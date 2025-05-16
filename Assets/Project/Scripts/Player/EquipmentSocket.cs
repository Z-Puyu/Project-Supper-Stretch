using UnityEngine;

namespace Project.Scripts.Player;

public class EquipmentSocket : MonoBehaviour {
    private GameObject? Equipment { get; set; }

    public void Attach(GameObject equipment) {
        if (this.Equipment != null) {
            Object.Destroy(this.Equipment);
        }

        this.Equipment = Object.Instantiate(equipment, this.transform);
    }
}
