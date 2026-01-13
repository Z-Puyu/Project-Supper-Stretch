using System;
using System.Threading;
using CommonFrameworks.Async;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GameplayAbilitiesSystem.Runtime.Animations {
    public sealed class AnimationController {
        private Animator Animator { get; set; }
        private CancellationTokenSource InternalInterrupter { get; set; } = new CancellationTokenSource();
        private PlayableGraph PlayableGraph { get; } = PlayableGraph.Create("Animation Graph");
        private AnimationPlayableOutput Output { get; set; }
        private AnimationMixerPlayable FinalMixer { get; set; }
        private AnimatorControllerPlayable AnimatorController { get; set; }
        private AnimationClipPlayable ActionAnimationClip { get; set; }
        internal event UnityAction<AnimationClip, UnityAction<AnimationNotifier>> OnAnimationStarted = delegate { };

        private AnimationController(Animator animator) {
            this.Animator = animator;
            this.FinalMixer = AnimationMixerPlayable.Create(this.PlayableGraph, 1);
            this.Output = AnimationPlayableOutput.Create(this.PlayableGraph, "Output", animator);
            this.AnimatorController = AnimatorControllerPlayable.Create(
                this.PlayableGraph, animator.runtimeAnimatorController
            );
        }

        internal static AnimationController Create(Animator animator) {
            AnimationController controller = new AnimationController(animator);
            controller.Output.SetSourcePlayable(controller.FinalMixer);
            controller.FinalMixer.DisconnectInput(0);
            controller.FinalMixer.ConnectInput(0, controller.AnimatorController, 0);
            controller.FinalMixer.SetInputWeight(0, 1);
            controller.PlayableGraph.Play();
            return controller;
        }

        internal void Destroy() {
            this.PlayableGraph.Destroy();
        }

        public async Awaitable<AnimationPlayResult> PlayActionAnimation(
            AnimationClip clip, UnityAction<AnimationNotifier> onNotify, Action? onInterrupt = null,
            CancellationToken interrupter = default
        ) {
            this.InterruptCurrentAction();
            this.FinalMixer.SetInputCount(2);
            this.ActionAnimationClip = AnimationClipPlayable.Create(this.PlayableGraph, clip);
            this.ActionAnimationClip.SetDuration(clip.length);
            this.ActionAnimationClip.SetApplyFootIK(false);
            this.ActionAnimationClip.SetPropagateSetTime(true);
            this.ActionAnimationClip.SetTime(0);
            this.FinalMixer.ConnectInput(1, this.ActionAnimationClip, 0);
            this.OnAnimationStarted.Invoke(clip, onNotify);
            using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(
                interrupter, this.InternalInterrupter.Token
            );

            try {
                await this.Crossfade(clip, cts.Token);
                return AnimationPlayResult.Ended;
            } catch (OperationCanceledException) {
                return AnimationPlayResult.Interrupted;
            } finally {
                this.ResetPlayableGraph();
            }
        }

        public void InterruptCurrentAction() {
            if (!this.ActionAnimationClip.IsValid()) {
                return;
            }

            this.InternalInterrupter.Cancel();
            this.InternalInterrupter.Dispose();
            this.InternalInterrupter = new CancellationTokenSource();
        }

        private void ResetPlayableGraph() {
            this.FinalMixer.SetInputWeight(0, 1);
            this.FinalMixer.SetInputWeight(1, 0);
            if (!this.ActionAnimationClip.IsValid()) {
                return;
            }

            this.FinalMixer.DisconnectInput(1);
            this.ActionAnimationClip.Destroy();
        }

        private async Awaitable Crossfade(
            AnimationClip anim, CancellationToken interrupter,
            float fadeRatio = 0.1f, float minFadeDurationInSeconds = 0.1f
        ) {
            minFadeDurationInSeconds = Mathf.Min(minFadeDurationInSeconds, anim.length / 2);
            float durationInSeconds = Mathf.Max(anim.length * fadeRatio, minFadeDurationInSeconds);
            float t = 0;
            while (t < durationInSeconds) {
                t += Time.deltaTime;
                float w = Mathf.SmoothStep(0, 1, t / durationInSeconds);
                this.FinalMixer.SetInputWeight(1, w);
                this.FinalMixer.SetInputWeight(0, 1 - w);
                await Awaitable.NextFrameAsync(interrupter);
            }

            await Awaitable.WaitForSecondsAsync(anim.length - durationInSeconds, interrupter);

            t = 0;
            while (!this.ActionAnimationClip.IsDone()) {
                t += Time.deltaTime;
                float w = Mathf.SmoothStep(1, 0, t / durationInSeconds);
                this.FinalMixer.SetInputWeight(1, w);
                this.FinalMixer.SetInputWeight(0, 1 - w);
                await Awaitable.NextFrameAsync(interrupter);
            }
        }
    }
}
