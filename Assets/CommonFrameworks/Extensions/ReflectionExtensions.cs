using System;
using System.Collections.Generic;
using System.Linq;
using CommonFrameworks.Collections;

namespace CommonFrameworks.Extensions {
    public static class ReflectionExtensions {
        private readonly record struct TypeReference(Type Type) {
            internal string AssemblyName { get; } = Type.Assembly.GetName().Name;
            internal string Namespace { get; } = Type.Namespace ?? string.Empty;
            internal string Name { get; } = Type.Name;
        }
        
        private readonly record struct Namespace(string Name) {
            internal int NumberOfParts => this.Name.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;
            internal string Root => string.IsNullOrWhiteSpace(this.Name) ? string.Empty : this.Name.Split('.')[0];
            
            internal static IEnumerable<Namespace> Denumerate(string @namespace) {
                string[] parts = @namespace.Split('.', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i <= parts.Length; i += 1) {
                    yield return new Namespace(string.Join(".", parts[..i]));
                }
            }

            internal bool IsImmediateChildOf(Namespace @namespace) {
                return this.Name.StartsWith(@namespace.Name) && this.NumberOfParts == @namespace.NumberOfParts + 1;
            }
            
            internal bool IsImmediateParentOf(Namespace @namespace) {
                return @namespace.IsImmediateChildOf(this);
            }
            
            public static implicit operator Namespace(string @namespace) => new Namespace(@namespace);
            public static implicit operator string(Namespace @namespace) => @namespace.Name;
        }
        
        public static Type Resolve(this Type type) {
            if (!type.IsGenericType) {
                return type;
            }
            
            Type res = type.GetGenericTypeDefinition();
            return res != type ? res : type;
        }

        private static void BuildTree(this Type type, IDictionary<string, ITree<string, Type>> trees) {
            Type self = type.Resolve();
            string name = $"{self.Assembly.GetName().Name}.{self.Namespace}.{self.Name}";
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
        }
    }
}
