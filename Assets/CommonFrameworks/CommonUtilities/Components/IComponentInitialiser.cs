using UnityEngine;

namespace CommonFrameworks.CommonUtilities.Components {
    public interface IComponentInitialiser<in T> where T : Component {
        public void Initialise(T component);
    }
}
