using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Scripts.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer<S> : MonoBehaviour where S : Enum {
    protected string ClipName(object _, int index) {
        string[] names = Enum.GetNames(typeof(S));
        return index < names.Length ? names[index] : "<color=red>Undefined";       
    }                                                                                                                                              
    
    [NotNull] protected AudioSource? RuntimeAudio { get; private set; }
    
    [field: SerializeField, RichLabel(nameof(this.ClipName), isCallback: true)] 
    protected List<AudioSource> AudioSources { get; private set; } = [];

    protected virtual void Awake() {
        this.RuntimeAudio = this.GetComponent<AudioSource>();
    }

    public void Play(S sound, float volume = 1) {
        int idx = (int)(object)sound;
        if (idx >= this.AudioSources.Count) {
            return;
        }

        if (this.AudioSources[idx].isPlaying) {
            return;       
        }
        
        this.AudioSources[idx].Stop();
        this.AudioSources[idx].volume = volume;
        this.AudioSources[idx].Play();
    }

    public void Pause(S sound) {
        int idx = (int)(object)sound;
        if (idx >= this.AudioSources.Count) {
            return;
        }
        
        this.AudioSources[idx].Stop();
    }
    
    public void Play(AudioResource clip, float volume = 1) {
        this.RuntimeAudio.Stop();
        this.RuntimeAudio.resource = clip;
        this.RuntimeAudio.volume = volume;
        this.RuntimeAudio.loop = false;
        this.RuntimeAudio.Play();
    }
}
