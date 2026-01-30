namespace GameCharacterBehaviours.Runtime.Movement {
    public interface ILocomotionStateMachine {
        public Locomotion.Gesture CurrentGesture { get; }
        public void Run();
        public void Walk();   
        public void Sprint();
    }
}
