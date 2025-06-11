using System;

namespace Project.Scripts.Util.Pooling;

public interface IPoolable<out T> where T : class {
    public abstract event Action<T> OnReturn;
}
