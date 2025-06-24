using System.Diagnostics.CodeAnalysis;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions;
using Project.Scripts.Common;
using Project.Scripts.Interaction;
using Project.Scripts.Items.InventorySystem;
using SaintsField;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Project.Scripts.Items.CraftingSystem;

[DisallowMultipleComponent, RequireComponent(typeof(InteractableObject), typeof(Workbench))]
public class CampFire : MonoBehaviour {
    public sealed record class UIData(Workbench Workbench, Inventory Inventory) : IPresentable {
        public string FormatAsText() {
            return string.Empty;
        }
    }
    
    public static event UnityAction<UIData> OnOpen = delegate { };
    public static event UnityAction<bool, Recipe> OnCraftStatusChecked = delegate { }; 
    public static event UnityAction<int> OnCraftCompleted = delegate { };
    
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
        this.Workbench.OnRecipeChanged += this.TestRecipe;
        this.Workbench.OnCraft += this.CraftSomething;
        this.Interactable.OnInteraction += this.Enter;
    }

    private void TestRecipe(int time, Recipe recipe) {
        CampFire.OnCraftStatusChecked.Invoke(time <= this.CampingDuration, recipe);
    }

    private void CraftSomething(int cookingTime, Item food) {
        this.CampingDuration -= cookingTime;
        this.PlayerInventory.Add(food);
        CampFire.OnCraftCompleted.Invoke(this.CampingDuration);
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
        
        CampFire.OnOpen.Invoke(new UIData(this.Workbench, this.PlayerInventory));
    }
}
