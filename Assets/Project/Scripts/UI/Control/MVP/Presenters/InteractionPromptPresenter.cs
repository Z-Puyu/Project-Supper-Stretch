using System;
using Project.Scripts.Interaction;
using Project.Scripts.UI.Control.MVP.Components;

namespace Project.Scripts.UI.Control.MVP.Presenters;

public class InteractionPromptPresenter : UIPresenter<InteractableObject, InteractionPromptView> {
    private void Awake() {
        this.View.Prompt = this.GetComponentInParent<InteractableObject>(includeInactive: true).Prompt;
        this.GetComponentInParent<InteractableObject>(includeInactive: true).OnActivated += this.Present;
    }

    private void Start() {
        this.View.Refresh();   
    }

    protected override void UpdateView(InteractableObject model) {
        this.View.Prompt = model.Prompt;   
    }
}