using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonFrameworks.Editor.Utils {
    public static class ReflectionExtensions {
        public static Type Resolve(this Type type) {
            if (!type.IsGenericType) {
                return type;
            }
            
            Type res = type.GetGenericTypeDefinition();
            return res != type ? res : type;
        }

        public static IEnumerable<Type> GetSubtypes(this Type type) {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(type.IsAssignableFrom);
        }
        
        public static IEnumerable<Type> GetSubclasses(this Type type) {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(subtype => subtype.IsSubclassOf(type));
        }
        
        public static IEnumerable<Type> GetConcreteSubclasses(this Type type) {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(subtype => subtype.IsSubclassOf(type) && !subtype.IsAbstract);
        }

        /*private static void BuildTree(this Type type, IDictionary<string, ITree<string, Type>> trees) {
            Type self = type.Resolve();
            string name = $"{self.Namespace}.{self.Name}";
            string[] parts = name.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (!trees.TryGetValue(parts[0], out ITree<string, Type>? tree)) {
                tree = Tree<string, Type>.CreateDirected(parts[0]);
                trees.Add(parts[0], tree);
            }

            string[] partials = parts.Select((_, i) => string.Join('.', parts[..^i]))
                                     .OrderBy(ns => ns.Length)
                                     .ToArray();
            for (int i = 0; i < partials.Length; i += 1) {
                tree.Add(partials[i]);
                if (i > 0) {
                    tree.Join(partials[i - 1], partials[i]);
                }
            }
            
            tree.Augment(partials[^1], type);
        }
        
        public static IEnumerable<ITree<string, Type>> GetSubtypes(this Type type) {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(assembly => assembly.GetTypes())
                                               .Where(type.IsAssignableFrom);
            IDictionary<string, ITree<string, Type>> trees = new Dictionary<string, ITree<string, Type>>();
            foreach (Type subtype in types) {
                subtype.BuildTree(trees);
            }
            
            return trees.Values;
        }
        
        public static IEnumerable<ITree<string, Type>> GetSubclasses(this Type type) {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(assembly => assembly.GetTypes())
                                               .Where(subtype => subtype.IsSubclassOf(type));
            IDictionary<string, ITree<string, Type>> trees = new Dictionary<string, ITree<string, Type>>();
            foreach (Type subtype in types) {
                subtype.BuildTree(trees);
            }
            
            return trees.Values;
        }
        
        public static IEnumerable<ITree<string, Type>> GetConcreteSubclasses(this Type type) {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(assembly => assembly.GetTypes())
                                               .Where(subtype => subtype.IsSubclassOf(type) && !subtype.IsAbstract);
            IDictionary<string, ITree<string, Type>> trees = new Dictionary<string, ITree<string, Type>>();
            foreach (Type subtype in types) {
                subtype.BuildTree(trees);
            }
            
            return trees.Values;
        }*/
    }
}
