using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SaintsField;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Audio;

[DisallowMultipleComponent, RequireComponent(typeof(AudioSource))]
public class PlayList : MonoBehaviour {
    [field: SerializeField] private List<AudioClip> Clips { get; set; } = [];
    
    [field: SerializeField, MinMaxSlider(0, 100)] 
    private Vector2 WaitTime { get; set; }

    [NotNull] private AudioSource? AudioSource { get; set; }
    private float NextClipStartTime { get; set; }
    private float OriginalVolume { get; set; }
    private bool IsPlaying { get; set; }

    private void Awake() {
        this.AudioSource = this.GetComponent<AudioSource>();
        this.OriginalVolume = this.AudioSource.volume;
    }

    public void PlayNext() {
        AudioClip clip = this.Clips[Random.Range(0, this.Clips.Count)];
        this.NextClipStartTime = Time.time + clip.length + Random.Range(this.WaitTime.x, this.WaitTime.y);
        this.AudioSource.PlayOneShot(clip);
        this.IsPlaying = true;
    }

    public void FadeInNext(float duration = 2f) {
        this.AudioSource.volume = 0;
        this.PlayNext();
        LeanTween.value(this.gameObject, this.SetVolume, 0, this.OriginalVolume, duration);
    }

    public void Stop() {
        this.IsPlaying = false;
        this.AudioSource.Stop();
        this.AudioSource.volume = this.OriginalVolume;
    }

    public void FadeOut(float duration = 2f) {
        LeanTween.value(this.gameObject, this.SetVolume, this.AudioSource.volume, 0, duration)
                 .setOnComplete(this.Stop);
    }
    
    private void SetVolume(float volume) {
        this.AudioSource.volume = volume;
    }

    private void Update() {
        if (!this.IsPlaying || Time.time < this.NextClipStartTime) {
            return;
        }
        
        this.PlayNext();
    }
}
