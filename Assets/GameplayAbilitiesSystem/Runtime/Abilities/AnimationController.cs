using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameplayAbilitiesSystem.Runtime.Abilities;

[DisallowMultipleComponent, RequireComponent(typeof(Animator))]
public sealed class AnimationController : MonoBehaviour {
    [NotNull] private Animator? Animator { get; set; } 
    private PlayableGraph PlayableGraph { get; } = PlayableGraph.Create("Animation Graph");
    private AnimationPlayableOutput Output { get; }
    private AnimationMixerPlayable FinalMixer { get; }
    private AnimatorControllerPlayable AnimatorController { get; set; }
    private AnimationClipPlayable ActionAnimationClip { get; set; }

    private AnimationController() {
        this.Output = AnimationPlayableOutput.Create(this.PlayableGraph, "Output", this.Animator);
        this.FinalMixer = AnimationMixerPlayable.Create(this.PlayableGraph, 1);
        this.Output.SetSourcePlayable(this.FinalMixer);
    }

    private void OnEnable() {
        this.AnimatorController = AnimatorControllerPlayable.Create(
            this.PlayableGraph, this.Animator.runtimeAnimatorController
        );
        
        this.FinalMixer.DisconnectInput(0);
        this.FinalMixer.ConnectInput(0, this.AnimatorController, 0);
        this.FinalMixer.SetInputWeight(0, 1);
    }

    private void Start() {
        this.PlayableGraph.Play();
    }

    public void PlayActionAnimation(AnimationClip clip) {
        if (this.ActionAnimationClip.IsValid() && this.ActionAnimationClip.GetAnimationClip() == clip) {
            return;
        }
        
        this.FinalMixer.SetInputCount(2);
        this.InterruptCurrentAction();
        
        this.ActionAnimationClip = AnimationClipPlayable.Create(this.PlayableGraph, clip);
        this.ActionAnimationClip.SetDuration(clip.length);
        this.ActionAnimationClip.SetApplyFootIK(false);
        this.ActionAnimationClip.SetPropagateSetTime(true);
        this.ActionAnimationClip.SetTime(0);
        
        this.FinalMixer.ConnectInput(1, this.ActionAnimationClip, 0);
        this.StartCoroutine(this.Crossfade(clip.length));
    }

    public void InterruptCurrentAction() {
        this.FinalMixer.SetInputWeight(0, 1);
        this.FinalMixer.SetInputWeight(1, 0);
        if (!this.ActionAnimationClip.IsValid()) {
            return;
        }

        this.FinalMixer.DisconnectInput(1);
        this.PlayableGraph.DestroyPlayable(this.ActionAnimationClip);
    }
    
    IEnumerator Crossfade(float clipLengthInSeconds, float fadeRatio = 0.1f, float minFadeDurationInSeconds = 0.1f) {
        float durationInSeconds = Mathf.Max(clipLengthInSeconds * fadeRatio, minFadeDurationInSeconds);
        float t = 0;
        while (t < durationInSeconds) {
            t += Time.deltaTime;
            float w = Mathf.SmoothStep(0, 1, t / durationInSeconds);
            this.FinalMixer.SetInputWeight(1, w);
            this.FinalMixer.SetInputWeight(0, 1 - w);
            yield return null;
        }

        yield return new WaitUntil(() => this.ActionAnimationClip.IsDone());
        this.FinalMixer.DisconnectInput(1);
        this.FinalMixer.SetInputCount(1);
        this.FinalMixer.SetInputWeight(0, 1);
        this.ActionAnimationClip.Destroy();
    }
}
