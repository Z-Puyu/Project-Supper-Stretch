using System;
using CommonFrameworks.Extensions;
using UnityEngine;

namespace CommonFrameworks.Utilities {
    public abstract class AnimatorStateBehaviour : StateMachineBehaviour {
        [Flags]
        private enum ExecutionTiming {
            None = 0,
            Enter = 1,
            Update = 1 << 1,
            Exit = 1 << 2,
            Move = 1 << 3,
            IK = 1 << 4
        }

        [SerializeField] private ExecutionTiming timing;
        
        protected abstract void Execute(Animator animator, AnimatorStateInfo state, int layer);

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (this.timing.Contains(ExecutionTiming.Enter)) {
                this.Execute(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (this.timing.Contains(ExecutionTiming.Update)) {
                this.Execute(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (this.timing.Contains(ExecutionTiming.Exit)) {
                this.Execute(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (this.timing.Contains(ExecutionTiming.Move)) {
                this.Execute(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (this.timing.Contains(ExecutionTiming.IK)) {
                this.Execute(animator, stateInfo, layerIndex);
            }
        }
    }
}
