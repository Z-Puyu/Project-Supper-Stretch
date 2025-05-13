using Project.Scripts.Events;
using UnityEngine.Events;

namespace Project.Scripts.Util.Observers;

public class Observer<T> {
    public event UnityAction<T, T> OnChange = delegate { };
    private T value;

    public T Value {
        get => this.value;
        set {
            if (!object.Equals(value, this.Value)) {
                this.OnChange.Invoke(this.value, value);
            }

            this.value = value;
        }
    }

    public Observer(T value) {
        this.value = value;
    }

    public static implicit operator T(Observer<T> observer) => observer.Value;
}

