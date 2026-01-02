using UnityEngine;

namespace CommonFrameworks.Components {
    public interface IComponentInitialiser<in T> where T : Component {
        public void Initialise(T component);
    }
}