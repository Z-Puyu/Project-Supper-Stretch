using CommonFrameworks.Extensions;
using SaintsField;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace CommonFrameworks.ObjectCreation;

[CreateAssetMenu(fileName = "New Poolable Object", menuName = "Object Pooling/Poolable Object")]
public sealed class PoolableObject : ScriptableObject {
    [field: SerializeField] private GameObject Prefab { get; set; }
    [field: SerializeField, MinValue(10)] private int PoolSize { get; set; } = 10;

    private Flyweight Create() {
        GameObject obj = Object.Instantiate(this.Prefab);
        obj.gameObject.SetActive(false);
        obj.name = $"{this.Prefab.name}";
        Flyweight flyweight = obj.GetOrAddComponent<Flyweight>();
        flyweight.SourceObject = this;
        return flyweight;
    }

    private static void Activate(Flyweight flyweight) {
        flyweight.gameObject.SetActive(true);
    }
        
    private static void Deactivate(Flyweight flyweight) {
        flyweight.gameObject.SetActive(false);
    }
        
    private static void Destroy(Flyweight flyweight) {
        Object.Destroy(flyweight.gameObject);
    }

    public void Return(Flyweight flyweight) {
        if (flyweight.SourceObject == this) {
            flyweight.ReturnToPool();
        } else {
#if DEBUG
            Debug.LogError($"{this.name} cannot return a flyweight which it did not create.", this);
#endif
        }
    }

    public void Return(GameObject obj) {
        if (obj.TryGetComponent(out Flyweight flyweight)) {
            this.Return(flyweight);
        } else {
#if DEBUG
            Debug.LogError($"{this.name} cannot return a game object which is not a flyweight.", this);
#endif  
        }
    }

    public void Return<T>(T instance) where T : Component {
        this.Return(instance.gameObject);
    }
        
    internal IObjectPool<Flyweight> CreatePool() {
        return new ObjectPool<Flyweight>(
            createFunc: this.Create,
            actionOnGet: PoolableObject.Activate,
            actionOnRelease: PoolableObject.Deactivate,
            actionOnDestroy: PoolableObject.Destroy,
            defaultCapacity: this.PoolSize
        );
    }
}