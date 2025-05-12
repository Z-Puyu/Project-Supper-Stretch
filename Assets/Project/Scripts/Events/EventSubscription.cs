using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.Events;

[Serializable]
public abstract class EventSubscription {
    public abstract void Enable();

    public abstract void Disable();
}

public abstract class EventSubscription<T> : EventSubscription {
    [field: SerializeField] private EventChannel<T>? Channel { get; set; }
    [field: SerializeField] private UnityEvent<GameEvent<T>>? OnEvent { get; set; }
    
    public override void Enable() {
        this.Channel?.Register(this);
    }
    
    public override void Disable() {
        this.Channel?.Unregister(this);
    }

    public void ReactTo(GameEvent<T> @event) {
        this.OnEvent?.Invoke(@event);
    }
}

[Serializable]
public class IntEventSubscription : EventSubscription<int>;

[Serializable]
public class StringEventSubscription : EventSubscription<string>;

[Serializable]
public class FloatEventSubscription : EventSubscription<float>;

[Serializable]
public class BoolEventSubscription : EventSubscription<bool>;

[Serializable]
public class Vector3EventSubscription : EventSubscription<Vector3>;

[Serializable]
public class Vector2EventSubscription : EventSubscription<Vector2>;

[Serializable]
public class GameObjectEventSubscription : EventSubscription<GameObject>;

[Serializable]
public class TransformEventSubscription : EventSubscription<Transform>;
