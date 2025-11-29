using UnityEngine;

namespace CommonFrameworks.CommonUtilities {
    public interface IComponentInitialiser<in T> where T : Component {
        public void Initialise(T component);
    }
}
