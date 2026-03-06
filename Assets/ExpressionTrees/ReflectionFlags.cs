using System.Reflection;

namespace ExpressionTrees {
    public static class ReflectionFlags {
        public const BindingFlags StaticMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public const BindingFlags InstanceMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags PublicMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        
        public const BindingFlags NonPublicMembers = BindingFlags.NonPublic | 
                                                     BindingFlags.Instance | 
                                                     BindingFlags.Static;

        public const BindingFlags Everything = BindingFlags.Public | 
                                               BindingFlags.NonPublic | 
                                               BindingFlags.Instance | 
                                               BindingFlags.Static;
    }
}
