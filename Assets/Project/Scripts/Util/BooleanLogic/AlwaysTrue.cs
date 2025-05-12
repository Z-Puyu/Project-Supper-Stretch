using System;

namespace Project.Scripts.Util.BooleanLogic;

[Serializable]
public class AlwaysTrue : ITestable {
    public bool Holds() {
        return true;
    }
}
