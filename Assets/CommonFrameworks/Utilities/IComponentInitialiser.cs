using UnityEngine;

namespace CommonFrameworks.Utilities;

public interface IComponentInitialiser<in T> where T : Component {
    public void Initialise(T component);
}