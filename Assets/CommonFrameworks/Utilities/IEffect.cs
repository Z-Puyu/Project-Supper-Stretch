namespace CommonFrameworks.Utilities {
    public interface IEffect<in T> {
        /// <summary>
        /// Applies the effect to the target.
        /// </summary>
        /// <param name="target">The target of the effect.</param>
        public void Apply(T target);
        
        /// <summary>
        /// Terminates the effect.
        /// </summary>
        /// <remarks>
        /// Call this method when you need to interrupt the effect or force the effect to stop.
        /// </remarks>
        public void Stop();
        
        /// <summary>
        /// Concludes the effect.
        /// </summary>
        /// <remarks>
        /// Call this method when the effect is completed only for cleaning up.
        /// </remarks>
        public void Complete();
    }

    public interface IEffect<in S, in T> {
        public void Apply(S source, T target);
        public void Stop();
    }
}