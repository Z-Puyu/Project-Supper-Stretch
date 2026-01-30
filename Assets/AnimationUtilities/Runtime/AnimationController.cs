using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CommonFrameworks.Components;
using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace AnimationUtilities.Runtime {
    [DisallowMultipleComponent]
    public sealed class AnimationController : Module {
        [NotNull] [field: SerializeField, Required] public Animator? Animator { get; private set; }
        [field: SerializeField] private RuntimeAnimatorController? RuntimeAnimatorController { get; set; }
        private CancellationTokenSource InternalInterrupter { get; set; } = new CancellationTokenSource();
        private PlayableGraph PlayableGraph { get; set; }
        private AnimationPlayableOutput Output { get; set; }
        private AnimationMixerPlayable FinalMixer { get; set; }
        private AnimatorControllerPlayable AnimatorController { get; set; }
        private AnimationClipPlayable ActionAnimationClip { get; set; }
        private HashSet<AnimationClip> PlaylistHistory { get; } = new HashSet<AnimationClip>();
        
        public event UnityAction<AnimationClip, UnityAction<AnimationNotifier>> OnAnimationStarted = delegate { };
        public event UnityAction<AnimationNotifier> OnNotified = delegate { };

        protected override void Awake() {
            base.Awake();
            if (!this.Animator) {
                if (this.TryGetComponentInChildren(out Animator? animator)) {
                    this.Animator = animator;
                } else if (this.transform.childCount > 0) {
                    this.Animator = this.transform.GetChild(0).gameObject.AddComponent<Animator>();
                } else {
                    this.Animator = this.AddSubobject<Animator>();
                }
            }
            
            if (this.RuntimeAnimatorController) {
                this.Animator.runtimeAnimatorController = this.RuntimeAnimatorController;
            }
            
            this.PlayableGraph = PlayableGraph.Create("Animation Graph");
            this.AnimatorController = AnimatorControllerPlayable.Create(
                this.PlayableGraph, this.Animator.runtimeAnimatorController
            );
            
            this.FinalMixer = AnimationMixerPlayable.Create(this.PlayableGraph, 1);
            this.FinalMixer.DisconnectInput(0);
            this.FinalMixer.ConnectInput(0, this.AnimatorController, 0);
            this.FinalMixer.SetInputWeight(0, 1);
            
            this.Output = AnimationPlayableOutput.Create(this.PlayableGraph, "Output", this.Animator);
            this.Output.SetSourcePlayable(this.FinalMixer);
            
            this.PlayableGraph.Play();
        }

        private void OnDestroy() {
            this.PlayableGraph.Destroy();
        }
        
        private void SendNotification(AnimationEvent @event) {
            this.OnNotified.Invoke((AnimationNotifier)@event.objectReferenceParameter);
        }

        public async Awaitable<AnimationPlayResult> Play(
            AnimationClip clip, UnityAction<AnimationNotifier> onNotify,
            CancellationToken interrupter = default
        ) {
            this.Interrupt();
            if (this.PlaylistHistory.Add(clip)) {
                foreach (AnimationEvent @event in clip.events) {
                    if (@event.objectReferenceParameter.GetType() != typeof(AnimationNotifier)) {
                        continue;
                    }
                    
                    @event.functionName = nameof(this.SendNotification);
                }    
            }
            
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

            this.OnNotified += onNotify;
            try {
                await this.Crossfade(clip, cts.Token);
                return AnimationPlayResult.Ended;
            } catch (OperationCanceledException) {
                return AnimationPlayResult.Interrupted;
            } finally {
                this.OnNotified -= onNotify;
                this.ResetPlayableGraph();
            }
        }

        public void Interrupt() {
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

        public void SetInteger(int hash, int value) {
            this.Animator.SetInteger(hash, value);
        }
        
        public void SetFloat(int hash, float value) {
            this.Animator.SetFloat(hash, value);
        }
        
        public void SetBool(int hash, bool value) {
            this.Animator.SetBool(hash, value);
        }
        
        public void SetTrigger(int hash) {
            this.Animator.SetTrigger(hash);
        }

        public void ResetTrigger(int hash) {
            this.Animator.ResetTrigger(hash);
        }
    }
}
