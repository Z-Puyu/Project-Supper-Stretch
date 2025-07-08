using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Project.Scripts.Common;
using Project.Scripts.Util.Linq;
using SaintsField;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.Scripts.Characters;

[DisallowMultipleComponent]
public class CharacterComponent : MonoBehaviour {
    private enum LifeCycle { ActiveOnDeath, DestroyOnDeath, AlwaysAlive }
    
    [field: SerializeField] private LifeCycle Policy { get; set; }
    
    [NotNull] public GameCharacter? Owner { get; private set; }
    
    [NotNull]
    [field: SerializeField, Required] 
    public Component? Component { get; private set; }
    
    [field: SerializeField] private Transform? Anchor { get; set; }
    private Vector3 PositionOffset { get; set; }

    private void Start() {
        this.Owner = this.GetComponentInParent<GameCharacter>();
        if (this.Anchor) {
            this.PositionOffset = this.transform.localPosition; 
        }
        
        if (!this.Owner) {
            Logging.Error("The character component is not attached to a game character", this);
            return;
        }
        
        switch (this.Policy) {
            case LifeCycle.ActiveOnDeath:
                this.Owner.OnKilled += this.WakeUp;
                this.gameObject.SetActive(false);
                break;
            case LifeCycle.DestroyOnDeath:
                this.Owner.OnKilled += () => Object.Destroy(this.gameObject);
                this.gameObject.SetActive(true);
                break;
            case LifeCycle.AlwaysAlive:
                this.Owner.OnKilled += () => this.GetComponents<Behaviour>()
                                                 .ForEach(behaviour => behaviour.enabled = false);
                break;
        }
    }

    private void WakeUp() { 
        this.gameObject.SetActive(true);
        this.GetComponentsInChildren<Behaviour>().Where(c => !c.enabled).ForEach(c => c.enabled = true);
        this.GetComponentsInChildren<Collider>().Where(c => !c.enabled).ForEach(c => c.enabled = true);
    }

    private void Update() {
        if (this.Anchor) {
            this.transform.position = this.Anchor.position + this.PositionOffset;
        }
    }
}
