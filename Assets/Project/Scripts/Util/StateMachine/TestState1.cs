using System;

namespace Project.Scripts.Util.StateMachine;

[Serializable]
public class TestState1 : State {
    public override void Enter() {
        throw new NotImplementedException();
    }
    public override void Exit() {
        throw new NotImplementedException();
    }
    public override void OnUpdate() {
        throw new NotImplementedException();
    }
    public override void OnFixedUpdate() {
        throw new NotImplementedException();
    }
}
