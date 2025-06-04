using System;
using UnityEngine.Events;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Project.Scripts.Util.Pooling;

public interface IPoolable<T> where T : class {
    public abstract event UnityAction<T> OnReturn;
}
