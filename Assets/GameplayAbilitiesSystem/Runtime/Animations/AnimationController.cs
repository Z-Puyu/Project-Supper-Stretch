using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    public sealed class AnimationController {
        private PlayableGraph PlayableGraph { get; } = PlayableGraph.Create("Animation Graph");
        private AnimationPlayableOutput Output { get; set; }
        private AnimationMixerPlayable FinalMixer { get; set; }
        private AnimatorControllerPlayable AnimatorController { get; set; }
        private AnimationClipPlayable ActionAnimationClip { get; set; }
        internal event UnityAction<AnimationClip> OnClipPlayed = delegate { };

        private AnimationController() {
            this.FinalMixer = AnimationMixerPlayable.Create(this.PlayableGraph, 1);
        }

        internal static AnimationController Create(Animator animator) {
            AnimationController controller = new AnimationController();
            controller.Output = AnimationPlayableOutput.Create(controller.PlayableGraph, "Output", animator);
            controller.Output.SetSourcePlayable(controller.FinalMixer);
            controller.AnimatorController = AnimatorControllerPlayable.Create(
                controller.PlayableGraph, animator.runtimeAnimatorController
            );

            controller.FinalMixer.DisconnectInput(0);
            controller.FinalMixer.ConnectInput(0, controller.AnimatorController, 0);
            controller.FinalMixer.SetInputWeight(0, 1);
            controller.PlayableGraph.Play();
            return controller;
        }

        public IEnumerator? PlayActionAnimation(AnimationClip clip) {
            if (this.ActionAnimationClip.IsValid() && this.ActionAnimationClip.GetAnimationClip() == clip) {
                return null;
            }

            this.FinalMixer.SetInputCount(2);
            this.InterruptCurrentAction();

            this.ActionAnimationClip = AnimationClipPlayable.Create(this.PlayableGraph, clip);
            this.ActionAnimationClip.SetDuration(clip.length);
            this.ActionAnimationClip.SetApplyFootIK(false);
            this.ActionAnimationClip.SetPropagateSetTime(true);
            this.ActionAnimationClip.SetTime(0);

            this.FinalMixer.ConnectInput(1, this.ActionAnimationClip, 0);
            return this.Crossfade(clip);
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

        private IEnumerator Crossfade(
            AnimationClip clip, float fadeRatio = 0.1f, float minFadeDurationInSeconds = 0.1f
        ) {
            float durationInSeconds = Mathf.Max(clip.length * fadeRatio, minFadeDurationInSeconds);
            float t = 0;
            while (t < durationInSeconds) {
                t += Time.deltaTime;
                float w = Mathf.SmoothStep(0, 1, t / durationInSeconds);
                this.FinalMixer.SetInputWeight(1, w);
                this.FinalMixer.SetInputWeight(0, 1 - w);
                yield return null;
            }

            this.OnClipPlayed(clip);
            yield return new WaitUntil(() => this.ActionAnimationClip.IsDone());
            this.FinalMixer.DisconnectInput(1);
            this.FinalMixer.SetInputCount(1);
            this.FinalMixer.SetInputWeight(0, 1);
            this.ActionAnimationClip.Destroy();
        }
    }
}
