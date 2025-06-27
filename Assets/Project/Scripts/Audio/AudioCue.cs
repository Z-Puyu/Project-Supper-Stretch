using System;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace Project.Scripts.Audio;

[Serializable]
public record struct AudioCue {
    [field: SerializeField, DefaultExpand] private List<AudioClip> Clips { get; set; }

    public bool Any(out AudioClip? clip) {
        if (this.Clips.Count == 0) {
            clip = null;
            return false;
        }
        
        clip = this.Clips[UnityEngine.Random.Range(0, this.Clips.Count)];
        return true;
    }
}
