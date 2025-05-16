using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.InteractionSystem.Detector;

public abstract class InteractorDetector : MonoBehaviour {
    public event UnityAction<Collider> OnDetection = delegate { };
    public event UnityAction<Collider> OnLostInteractor = delegate { };

    protected void Detected(Collider interactor) {
        this.OnDetection.Invoke(interactor);
    }
    
    protected void LoseInteractor(Collider interactor) {
        this.OnLostInteractor.Invoke(interactor);
    }
}
