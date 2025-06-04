using System;
using UnityEngine;

namespace Project.Scripts.Util.ChainOfResponsibilities;

public interface IProcessor<in T> {
    public abstract void Process(T input);
}

public interface IProcessor<in T, out S> {
    public abstract S Process(T input);
}
