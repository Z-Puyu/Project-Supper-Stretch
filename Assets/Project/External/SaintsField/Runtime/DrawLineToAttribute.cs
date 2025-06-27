using System;
using System.Diagnostics;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DrawLineToAttribute: DrawLineAttribute
    {
        public DrawLineToAttribute(
            string target = null, int targetIndex = 0, string targetSpace = "this",
            string space = "this",
            EColor eColor = EColor.White, float alpha = 1f,  string color = null,
            float dotted = -1f
        ) : base(null, 0, space, target, targetIndex, targetSpace, eColor, alpha, color, dotted)
        {
        }
    }
}
