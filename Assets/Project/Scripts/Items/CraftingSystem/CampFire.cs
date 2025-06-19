using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.Common.UI;
using Project.Scripts.Interaction;
using Project.Scripts.Items.InventorySystem;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent, RequireComponent(typeof(InteractableObject), typeof(Workbench))]
public class CampFire : MonoBehaviour {
    public static event UnityAction<UIData<(Workbench workbench, Inventory inventory)>> OnOpen = delegate { };
    public static event UnityAction<bool> OnCookingStatusChecked = delegate { }; 
    
    [NotNull] private AttributeSet? PlayerAttributes { get; set; }
    [NotNull] private Inventory? PlayerInventory { get; set; }
    [NotNull] private InteractableObject? Interactable { get; set; }
    [NotNull] private Workbench? Workbench { get; set; }
    private bool HasBeenUsedBefore { get; set; }
    
    [field: SerializeField, PropRange(0, 24)] 
    private int CampingDuration { get; set; } = 12;

    [field: SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int HealthPerHourOfRest { get; set; } = new Vector2Int(5, 10);
    
    [field: SerializeField]
    private GameplayEffect? RestEffect { get; set; }
    
    private void Awake() {
        this.Interactable = this.GetComponent<InteractableObject>();
        this.Workbench = this.GetComponent<Workbench>();
    }

    private void Start() {
        this.Workbench.OnRecipeChanged += this.AdjustRecipe;
        this.Workbench.OnCraft += this.CraftSomething;
        this.Interactable.OnInteraction += this.Enter;
    }

    private void AdjustRecipe(Recipe recipe) {
        CampFire.OnCookingStatusChecked.Invoke(recipe.CookingTime <= this.CampingDuration);
    }

    private void CraftSomething(int cookingTime, Item food) {
        this.CampingDuration -= cookingTime;
        this.PlayerInventory.Add(food);
    }

    private void Rest() {
        int hp = 0;
        for (int i = 0; i < this.CampingDuration; i += 1) {
            hp += Random.Range(this.HealthPerHourOfRest.x, this.HealthPerHourOfRest.y);
        }

        if (!this.RestEffect) {
            return;
        }

        GameplayEffectExecutionArgs args = new GameplayEffectExecutionArgs.Builder().WithNumericalData("RestAmount", hp)
                                                                                    .Build();
        this.PlayerAttributes.AddEffect(this.RestEffect, args);
    }

    private void Enter(Interactor interactor) {
        if (!interactor.CompareTag("Player")) {
            return;
        }

        if (!this.HasBeenUsedBefore) {
            this.PlayerAttributes = interactor.GetComponent<AttributeSet>();
            this.PlayerInventory = interactor.GetComponent<Inventory>();
            this.HasBeenUsedBefore = true;
        }
        
        Debug.Log($"{interactor.gameObject.name} opened campfire.");
        CampFire.OnOpen.Invoke(
            new UIData<(Workbench workbench, Inventory inventory)>((this.Workbench, this.PlayerInventory)));
    }
}
