using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Audio;

[RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
public class AudioPlayer<S> : MonoBehaviour where S : Enum {
    [field: SerializeField, RichLabel(nameof(this.ClipName), isCallback: true)]
    private List<AudioCue> Clips { get; set; } = [];

    private string ClipName(object _, int index) {
        string[] names = Enum.GetNames(typeof(S));
        return index < names.Length ? names[index] : "<color=red>Undefined";       
    }
    
    [NotNull] private AudioSource? AudioSource { get; set; }

    private void Awake() {
        this.AudioSource = this.GetComponent<AudioSource>();
    }
    
    public void Play(S sound, float volume = 1) {
        if (this.Clips[(int)(object)sound].Any(out AudioClip? clip)) {
            this.AudioSource.PlayOneShot(clip, volume);
        }
    }
}
