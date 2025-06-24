using UnityEngine;

namespace Project.Scripts.Common;

[DisallowMultipleComponent]
public class PlayerStart : MonoBehaviour {
    [field: SerializeField] private GameObject? PlayerPrefab { get; set; }

    private void Start() {
        GameObject? player = Object.Instantiate(this.PlayerPrefab, this.transform);
        if (player) {
            player.transform.SetParent(null);
        }
    }
}