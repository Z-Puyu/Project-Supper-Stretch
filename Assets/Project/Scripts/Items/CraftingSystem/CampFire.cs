using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Common;
using Project.Scripts.Interaction;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Components;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent, RequireComponent(typeof(InteractableObject), typeof(Workbench))]
public class CampFire : MonoBehaviour {
    public sealed record class UIData(Workbench Workbench, Inventory Inventory, int RemainingTime, int CraftDuration) : IPresentable {
        public string FormatAsText() {
            return string.Empty;
        }
    }
    
    public static event UnityAction<UIData> OnCampingSituationUpdated = delegate { }; 
    
    public static event UnityAction<UIData> OnOpen = delegate { };
    
    [field: SerializeField] private GameplayEffect? FirstUseEffect { get; set; }
    [NotNull] private AttributeSet? PlayerAttributes { get; set; }
    [NotNull] private Inventory? PlayerInventory { get; set; }
    [NotNull] private InteractableObject? Interactable { get; set; }
    [NotNull] private Workbench? Workbench { get; set; }
    private bool HasBeenUsedBefore { get; set; }
    
    [field: SerializeField, PropRange(0, 24)] 
    private int CampingDuration { get; set; } = 12;
    
    private int RemainingTime { get; set; }
    
    private void Awake() {
        this.Interactable = this.GetComponent<InteractableObject>();
        this.Workbench = this.GetComponent<Workbench>();
        this.RemainingTime = this.CampingDuration;
    }

    private void Start() {
        this.Workbench.OnRecipeChanged += this.UpdateRecipe;
        this.Workbench.OnCraft += this.CollectProduct;
        this.Interactable.OnInteraction += this.Enter;
    }

    private void UpdateRecipe(int duration, Recipe recipe) {
        CampFire.OnCampingSituationUpdated
                .Invoke(new UIData(this.Workbench, this.PlayerInventory, this.RemainingTime, duration));
    }
    
    private void CollectProduct(int timeConsumed, Item item) {
        this.RemainingTime -= timeConsumed;
        if (this.RemainingTime < 0) {
            Logging.Error("Camping duration exceeded.", this);
            return;
        }
        
        this.PlayerInventory.Add(item);
        CampFire.OnCampingSituationUpdated
                .Invoke(new UIData(this.Workbench, this.PlayerInventory, this.RemainingTime, 0));
    }

    private void Enter(Interactor interactor) {
        if (!interactor.CompareTag("Player")) {
            return;
        }

        if (!this.HasBeenUsedBefore) {
            this.PlayerAttributes = interactor.GetSiblingComponent<AttributeSet>();
            this.PlayerInventory = interactor.GetSiblingComponent<Inventory>();
            this.HasBeenUsedBefore = true;
            if (this.FirstUseEffect) {
                this.PlayerAttributes.AddEffect(this.FirstUseEffect);
            }
        }
        
        CampFire.OnOpen.Invoke(new UIData(this.Workbench, this.PlayerInventory, this.RemainingTime, 0));
    }
}
