using System;
using System.Collections.Generic;
using Project.Scripts.Util.Visitor;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.AttributeSystem.Modifiers;

public sealed 
        class ModifierManager : IVisitable<ModifierManager> {
    private AttributeSet AttributeSet { get; init; }
    private Dictionary<AttributeType, LinkedList<Modifier>> Modifiers { get; init; } = [];
    private Dictionary<AttributeType, Vector2> FinalValueModifications { get; init; } = [];
    private Dictionary<Modifier, Stack<LinkedListNode<Modifier>>> Nodes { get; init; } = [];
    public event UnityAction<AttributeType> OnModifierUpdate = delegate { }; 
    
    public ModifierManager(AttributeSet attributeSet) {
        this.AttributeSet = attributeSet;
    }
    
    private void AddModifierNode(Modifier modifier, LinkedListNode<Modifier> node) {
        if (!this.Nodes.TryGetValue(modifier, out Stack<LinkedListNode<Modifier>> nodes)) {
            Stack<LinkedListNode<Modifier>> stack = [];
            stack.Push(node);
            this.Nodes.Add(modifier, stack);
        } else {
            nodes.Push(node);
        }
        
        this.OnModifierUpdate.Invoke(modifier.Target);
    }
    
    private LinkedListNode<Modifier> PopNode(Modifier modifier) {
        if (!this.Nodes.TryGetValue(modifier, out Stack<LinkedListNode<Modifier>> nodes)) {
            throw new ArgumentException();
        }
        
        LinkedListNode<Modifier> node = nodes.Pop();
        if (nodes.Count == 0) {
            this.Nodes.Remove(modifier);
        }

        return node;
    }

    public (float baseOffset, float baseMultiplier) AggregateBaseValueModifiers(AttributeType attribute) {
        if (!this.Modifiers.TryGetValue(attribute, out LinkedList<Modifier> list)) {
            return (0, 1);
        }
        
        float baseOffset = 0;
        float baseMultiplier = 0;
        foreach (Modifier modifier in list) {
            modifier.AggregateTo(this.AttributeSet, ref baseOffset, ref baseMultiplier);
        }
        
        return (baseOffset, Mathf.Max(0, 1 + baseMultiplier / 100.0f));
    }
    
    public (float finalOffset, float finalMultiplier) AggregateFinalValueModifiers(AttributeType attribute) {
        return !this.FinalValueModifications.TryGetValue(attribute, out Vector2 vector)
                ? (0, 1)
                : (vector.x, Mathf.Max(0, 1 + vector.y / 100.0f));
    }

    public void AddAsFinalValueModifier(Modifier modifier) {
        if (this.FinalValueModifications.ContainsKey(modifier.Target)) {
            this.FinalValueModifications[modifier.Target] += modifier.ToFinalValueModification(this.AttributeSet);
        } else {
            this.FinalValueModifications.Add(modifier.Target, modifier.ToFinalValueModification(this.AttributeSet));
        }
        
        this.OnModifierUpdate.Invoke(modifier.Target);
    }

    public void AddAsBaseValueMultiplier(Modifier modifier) {
        if (!this.Modifiers.TryGetValue(modifier.Target, out LinkedList<Modifier> modifiers)) {
            LinkedList<Modifier> container = [];
            this.AddModifierNode(modifier, container.AddLast(modifier));
            this.Modifiers.Add(modifier.Target, container);
        } else {
            this.AddModifierNode(modifier, modifiers.AddLast(modifier));
        }
    }

    public void AddAsBaseValueOffset(Modifier modifier) {
        if (!this.Modifiers.TryGetValue(modifier.Target, out LinkedList<Modifier> modifiers)) {
            LinkedList<Modifier> container = [];
            this.AddModifierNode(modifier, container.AddFirst(modifier));
            this.Modifiers.Add(modifier.Target, container);
        } else {
            this.AddModifierNode(modifier, modifiers.AddFirst(modifier));
        }
    }
    
    public void RemoveModifier(Modifier modifier) {
        if (!this.Modifiers.TryGetValue(modifier.Target, out LinkedList<Modifier> modifiers)) {
            return;
        }

        modifiers.Remove(this.PopNode(modifier));
        this.OnModifierUpdate.Invoke(modifier.Target);
    }

    public void Accept(IVisitor<ModifierManager> visitor) {
        visitor.Visit(this);
    }
}
