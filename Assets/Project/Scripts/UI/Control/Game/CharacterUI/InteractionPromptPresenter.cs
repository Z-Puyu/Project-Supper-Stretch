using Project.Scripts.Common.UI;
using Project.Scripts.Interaction;
using Project.Scripts.UI.Components;

namespace Project.Scripts.UI.Control.Game.CharacterUI;

public class InteractionPromptPresenter : UIPresenter<InteractableObject, InteractionPrompt, UIData<string>> {
    private void Awake() {
        if (!this.Model) {
            this.Model = this.GetComponentInParent<InteractableObject>();
        }
    }

    private void Start() {
        this.Refresh();
    }

    public override void Present(UIData<string> data) {
        this.View.Display(data);
    }

    public override void Refresh() {
        this.Present(this.Model ? this.Model.Prompt : "Interact");
    }
}