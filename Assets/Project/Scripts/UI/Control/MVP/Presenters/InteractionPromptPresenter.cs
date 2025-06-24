using Project.Scripts.Interaction;
using Project.Scripts.UI.Control.MVP.Components;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class InteractionPromptPresenter : UIPresenter<InteractableObject, InteractionPromptView> {
    private void Awake() {
        this.GetComponentInParent<InteractableObject>().OnActivated += this.Present;
    }

    protected override void UpdateView(InteractableObject model) {
        this.View.Prompt = model.Prompt;   
    }
}