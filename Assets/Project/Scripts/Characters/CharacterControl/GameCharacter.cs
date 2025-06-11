using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.AttributeTypes;
using Project.Scripts.AttributeSystem.GameplayEffects;
using Project.Scripts.AttributeSystem.GameplayEffects.Executions.Custom;
using Project.Scripts.AttributeSystem.Modifiers;
using Project.Scripts.Characters.CharacterControl.Combat;
using Project.Scripts.InventorySystem;
using Project.Scripts.Items;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Util.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters.CharacterControl;

[DisallowMultipleComponent, RequireComponent(typeof(AttributeSet), typeof(Health))]
public abstract class GameCharacter : MonoBehaviour;

public abstract class GameCharacter<C> : GameCharacter, IConsumer where C : CharacterData {
    [field: SerializeField] protected C? CharacterData { get; set; }
    [NotNull] protected Health? Health { get; private set; }
    [NotNull] protected AttributeSet? AttributeSet { get; private set; }
    [NotNull] protected Inventory? Inventory { get; private set; }
    [NotNull] protected EquipmentSystem? EquipmentSystem { get; private set; }
    [NotNull] protected CharacterMovement? Movement { get; private set; }

    protected virtual void Awake() {
        this.AttributeSet = this.GetComponent<AttributeSet>();
        this.Health = this.GetComponent<Health>();
        this.Movement = this.gameObject.GetComponent<CharacterMovement>();
        
        this.Inventory = this.gameObject.AddComponent<Inventory>();
        this.EquipmentSystem = this.gameObject.AddComponent<EquipmentSystem>();
    }

    public virtual void Initialise() {
        this.InitialiseAttributes();
        this.InitialiseHealth();
    }

    private void InitialiseHealth() {
        this.Health.Initialise();
        this.Health.TakingDamageAction = damaged;
        this.Health.OnDeath += _ => Object.Destroy(this.gameObject);
        return;
        
        void damaged(AttributeSet instigator) {
            GameplayEffectFactory.CreateInstant<TakingDamage>()
                                 .Execute(new WeaponDamageExecutionArgs(this.AttributeSet, instigator));
        }
    }

    private void InitialiseAttributes() {
        if (!this.CharacterData) {
            return;
        }
        
        foreach (CharacterAttributeData data in this.CharacterData.Attributes) {
            if (data.IsCappedByValue) {
                this.AttributeSet.Init(data.AttributeType, data.Value, maxValue: data.MaxValue);
            } else if (data.IsCappedByAttribute) {
                this.AttributeSet.Init(data.Cap, data.Value);
                this.AttributeSet.Init(data.AttributeType, data.Value, cap: data.Cap);
            } else {
                this.AttributeSet.Init(data.AttributeType, data.Value);
            }
        }
    }

    public void Consume(in Inventory from, in Item item, int count = 1) {
        switch (item.Type) {
            case ItemType.Drink or ItemType.Food or ItemType.FoodIngredient or ItemType.Potion:
                for (int i = 0; i < count; i += 1) {
                    item.Properties.SelectMany(onApply).ForEach(this.AttributeSet.AddModifier);
                    from.Remove(item);
                }
                
                break;
            case ItemType.Equipment:
                if (this.EquipmentSystem.Has(item)) {
                    this.EquipmentSystem.Unequip(item);
                    item.Properties.SelectMany(onRevoke).ForEach(this.AttributeSet.AddModifier);
                } else {
                    this.EquipmentSystem.Equip(item);
                    item.Properties.SelectMany(onApply).ForEach(this.AttributeSet.AddModifier);
                }

                break;
            default:
                Debug.Log($"Cannot use {item.Type} item {item.Name}.");
                break;
        }

        return;
        IEnumerable<Modifier> onApply(IItemProperties props) => props.ApplyOn(this.AttributeSet);
        IEnumerable<Modifier> onRevoke(IItemProperties props) => props.RevokeFrom(this.AttributeSet);
    }
}
