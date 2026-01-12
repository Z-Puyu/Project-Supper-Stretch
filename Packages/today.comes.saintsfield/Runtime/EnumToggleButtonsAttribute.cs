using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumToggleButtonsAttribute: PathedDropdownAttribute
    {
        public EnumToggleButtonsAttribute()
        {
        }

        [Obsolete("Use [DefaultExpand] instead")]
        public EnumToggleButtonsAttribute(bool defaultExpanded)
        {
        }
    }
}
