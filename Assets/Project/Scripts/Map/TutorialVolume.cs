using Project.Scripts.Common;
using UnityEngine;

namespace Project.Scripts.Map;

public class TutorialVolume : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (!other.transform.root.CompareTag("Player")) {
            return;
        }

        GameEvents.UI.OnNextTutorial?.Invoke();     
        Object.Destroy(this.gameObject);
    }
}
