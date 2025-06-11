using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumFlagsAttribute: EnumToggleButtonsAttribute
    {
        public EnumFlagsAttribute() : base()
        {
        }

        [Obsolete("Use [DefaultExpand] instead")]
        public EnumFlagsAttribute(bool defaultExpanded) : base(defaultExpanded)
        {
        }
    }
}
