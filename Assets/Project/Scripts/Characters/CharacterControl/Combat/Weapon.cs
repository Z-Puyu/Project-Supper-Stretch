using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using Project.Scripts.AttributeSystem.Attributes;
using Project.Scripts.AttributeSystem.Attributes.Definitions;
using Project.Scripts.Util.Linq;
using UnityEngine;

namespace Project.Scripts.Characters.CharacterControl.Combat;

[DisallowMultipleComponent]
public class Weapon : MonoBehaviour, IDamageDealer {
    private AdvancedDropdownList<AttributeKey> AllAccessibleAttributes =>
            this.GetComponent<IAttributeReader>().AllAccessibleAttributes;
    
    [NotNull] 
    [field: SerializeField, Required] 
    private HitBox? HitBox { get; set; }
    
    [field: SerializeField, AdvancedDropdown(nameof(this.AllAccessibleAttributes))]
    private AttributeKey BaseDamageAttribute { get; set; }
    
    [NotNull] private IAttributeReader? AttributeReader { get; set; }

    private void Awake() {
        this.AttributeReader = this.GetComponent<IAttributeReader>();
    }
    
    private void Start() {
        this.HitBox.OnHit += this.Damage;
    }

    public void Damage(IEnumerable<IDamageable> targets) {
        targets.ForEach(this.Damage);
    }

    public void Damage(IDamageable target) {
        target.TakeDamage(this.AttributeReader.ReadCurrent(this.BaseDamageAttribute.FullName), this.gameObject);
    }
}
