using Project.Scripts.InteractionSystem;
using Project.Scripts.UI.Components;
using Project.Scripts.UI.Control;

namespace Project.Scripts.UI.CharacterUI;

public class InteractionPromptPresenter : UIPresenter<InteractionPrompt, InteractableObject> {
    protected override void Awake() {
        base.Awake();
        if (!this.Model) {
            this.Model = this.GetComponentInParent<InteractableObject>();
        }
    }

    private void Start() {
        this.Model!.OnActivated += this.Present;
    }

    public override void Present(object data) {
        this.View.Display(data);
    }
    
    public override void Present() {
        this.View.Display("Interact");
    }
}