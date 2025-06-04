using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.ObjectDetection;

public abstract class DetectionZone : MonoBehaviour {
    public event UnityAction<Collider> OnDetection = delegate { };
    public event UnityAction<Collider> OnLostSight = delegate { };

    protected void Detected(Collider interactor) {
        this.OnDetection.Invoke(interactor);
    }
    
    protected void LoseTarget(Collider interactor) {
        this.OnLostSight.Invoke(interactor);
    }
}
