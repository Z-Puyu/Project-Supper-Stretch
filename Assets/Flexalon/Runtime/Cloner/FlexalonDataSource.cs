using System;
using System.Collections.Generic;

namespace Flexalon.Runtime.Cloner
{
    /// <summary> Provides data for a FlexalonCloner. </summary>
    public interface DataSource
    {
        /// <summary> For each element, FlexalonCloner will instantiate a new gameObject. </summary>
        IReadOnlyList<object> Data { get; }

        /// <summary> Invoke to notify FlexalonCloner that the data has changed. </summary>
        event Action DataChanged;
    }
}