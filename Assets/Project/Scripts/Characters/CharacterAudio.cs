using System;
using Project.Scripts.Audio;
using Project.Scripts.Characters.Combat;
using Project.Scripts.Common;
using Project.Scripts.Interaction.ObjectDetection;
using Project.Scripts.Items.Equipments;
using Project.Scripts.Items.InventorySystem;
using Project.Scripts.Util.Components;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Project.Scripts.Characters;

public class CharacterAudio : AudioPlayer<CharacterAudio.Sound> {
    public enum Sound {
        Hurt,
        Hit,
        BattleCry,
        Idle,
        Death,
        Attack,
        Equip,
        Unequip,
        ConsumeFood,
        Starving
    }

    [field: SerializeField, Required] private GameCharacter? Character { get; set; }
    
    protected override void Awake() {
        base.Awake();
        if (!this.Character) {
            Logging.Error($"{this.name} has no character!", this);
            return;
        }
        
        this.Character.GetComponentsInChildren<HitBox>().ForEach(hitbox => hitbox.OnHit += (_, _) => this.Play(Sound.Hit));
        this.Character.HealthComponent!.OnDamaged += _ => {
            if (Random.Range(0, 2) == 0) {
                this.Play(Sound.Hurt);
            }
        };
        this.Character.HealthComponent.OnDeath += _ => {
            this.AudioSources.ForEach(source => source.Stop());
            this.Play(Sound.Death);
        };

        if (this.Character.HasChildComponent(out Inventory inventory)) {
            inventory.OnItemConsumed += () => this.Play(Sound.ConsumeFood);
        }

        if (this.Character.HasChildComponent(out EquipmentSet equipment)) {
            equipment.OnEquip += () => this.Play(Sound.Equip);
            equipment.OnUnequip += () => this.Play(Sound.Unequip);
        }
    }
}
