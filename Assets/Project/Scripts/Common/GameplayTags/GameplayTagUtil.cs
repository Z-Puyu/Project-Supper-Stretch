using System;
using System.Collections.Generic;
using System.Linq;
using SaintsField;

namespace Project.Scripts.Common.GameplayTags;

public static class GameplayTagUtil {
    public static void TracePath<S, T>(this T origin) where T : GameplayTagNode where S : GameplayTagTree<T> {
        string path = string.Empty;
        foreach (S tree in GameplayTagTree<T>.Instances.OfType<S>()) {
            Dictionary<GameplayTagNode, GameplayTagNode> predecessors = [];
            Stack<string> address = [];
            PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
            iterator.ForEachChild = (parent, child) => predecessors.Add(child, parent);
            iterator.ShouldStop = node => node == origin;
            iterator.OnEnd = backtrack;
            tree.Traverse(iterator);
            if (path != string.Empty) {
                origin.Namespace = tree.Namespace;
                origin.Path = path;
                break;
            }
            
            continue;

            void backtrack(GameplayTagNode last) {
                if (last != origin) {
                    return;
                }
                
                address.Push(last.Tag);
                while (predecessors.TryGetValue(last, out GameplayTagNode parent)) {
                    address.Push(parent.Tag);
                    last = parent;
                }
                    
                path = string.Join('.', address);
            }
        }

        if (path == string.Empty) {
            throw new ArgumentException($"Could not find path for {origin.Tag}");
        }
    }
    
    private static AdvancedDropdownList<string> FindLeaves<T>(this GameplayTagTree<T> tree) where T : GameplayTagNode {
        Dictionary<string, AdvancedDropdownList<string>> sections = [];
        PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
        iterator.ForEach = makeSection;
        iterator.ForEachChild = (parent, child) => sections[parent.Path].Add(sections[child.Path]);
        tree.Traverse(iterator);
        return new AdvancedDropdownList<string>(tree.Namespace,
            tree.Nodes.Where(node => node is not null).Select(node => sections[node.Path]));

        void makeSection(T node) {
            AdvancedDropdownList<string> section = node.Children.Count > 0
                    ? new AdvancedDropdownList<string>(node.Tag)
                    : new AdvancedDropdownList<string>(node.Tag, $"{tree.Namespace}.{node.Path}");
            sections.TryAdd(node.Path, section);
        }
    }

    public static AdvancedDropdownList<string> LeafTags<T>(this IEnumerable<GameplayTagTree<T>> trees)
            where T : GameplayTagNode {
        return new AdvancedDropdownList<string>("Namespaces", trees.Select(tree => tree.FindLeaves()));
    }

    public static AdvancedDropdownList<string> AllTags<T>(this GameplayTagTree<T> tree) where T : GameplayTagNode {
        AdvancedDropdownList<string> list = new AdvancedDropdownList<string>(tree.Namespace);
        List<string> tags = [];
        PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
        iterator.ForEach = tag => tags.Add(tag.Path);
        tree.Traverse(iterator);
        foreach (string key in tags) {
            list.Add(key, $"{tree.Namespace}.{key}");
        }

        return list;
    }

    public static AdvancedDropdownList<string> AllTags<T>(this IEnumerable<GameplayTagTree<T>> trees)
            where T : GameplayTagNode {
        AdvancedDropdownList<string> list = new AdvancedDropdownList<string>("Namespaces");
        foreach (GameplayTagTree<T> tree in trees.Where(def => def.Nodes.Count > 0)) {
            AdvancedDropdownList<string> section = new AdvancedDropdownList<string>(tree.Namespace);
            list.Add(section);
            List<string> tags = [];
            PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
            iterator.ForEach = tag => tags.Add(tag.Path);
            tree.Traverse(iterator);
            foreach (string key in tags.Where(tag => tag != string.Empty)) {
                section.Add(key, $"{tree.Namespace}.{key}");
            }
        }

        return list;
    }
    
    private static AdvancedDropdownList<T> LeafNodes<T>(this GameplayTagTree<T> tree) where T : GameplayTagNode {
        Dictionary<string, AdvancedDropdownList<T>> sections = [];
        PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
        iterator.ForEach = makeSection;
        iterator.ForEachChild = (parent, child) => sections[parent.Path].Add(sections[child.Path]);
        tree.Traverse(iterator);
        return new AdvancedDropdownList<T>(tree.Namespace,
            tree.Nodes.Where(node => node is not null).Select(node => sections[node.Path]));

        void makeSection(T node) {
            AdvancedDropdownList<T> section = node.Children.Count > 0
                    ? new AdvancedDropdownList<T>(node.Tag)
                    : new AdvancedDropdownList<T>(node.Tag, node);
            sections.TryAdd(node.Tag, section);
        }
    }

    public static AdvancedDropdownList<T> LeafNodes<T>(this IEnumerable<GameplayTagTree<T>> trees)
            where T : GameplayTagNode {
        return new AdvancedDropdownList<T>("Namespaces", trees.Select(tree => tree.LeafNodes()));
    }

    public static AdvancedDropdownList<T> AllNodes<T>(this IEnumerable<GameplayTagTree<T>> trees)
            where T : GameplayTagNode {
        AdvancedDropdownList<T> list = new AdvancedDropdownList<T>("Namespaces");
        foreach (GameplayTagTree<T> tree in trees.Where(t => t.Nodes.Count(node => node is not null) > 0)) {
            AdvancedDropdownList<T> section = new AdvancedDropdownList<T>(tree.Namespace);
            list.Add(section);
            List<T> tags = [];
            PreorderIterator<T> iterator = new PreorderIterator<T>(tree.Nodes);
            iterator.ForEach = node => tags.Add(node);
            tree.Traverse(iterator);
            foreach (T node in tags) {
                section.Add(node.Path, node);
            }
        }

        return list;
    }

    public static T? Definition<S, T>(this string name) where T : GameplayTagNode where S : GameplayTagTree<T> {
        foreach (S tree in GameplayTagTree<T>.Instances.OfType<S>()) {
            if (tree.TryFind(name, out T? node)) {
                return node;
            }
        }

        return null;
    }
}
