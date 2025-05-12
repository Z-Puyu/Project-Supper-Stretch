using System;

namespace Project.Scripts.Util.BooleanLogic;

[Serializable]
public class AlwaysFalse : ITestable {
    public bool Holds() {
        return false;
    }
}
