using System;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Interaction.MapObjects;

[DisallowMultipleComponent, RequireComponent(typeof(InteractableObject))]
public class TreasureChest : MonoBehaviour {
    [NotNull]
    [field: SerializeField, Required]
    private AudioSource? OpeningAudio { get; set; }
    
    private void Start() {
        this.GetComponent<InteractableObject>().OnInteraction += _ => this.OpeningAudio.Play();       
    }
}
