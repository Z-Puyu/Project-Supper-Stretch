using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Project.Scripts.Util.Pooling;

public class Pool<T> : IObjectPool<T> where T : MonoBehaviour, IPoolable<T> {
    private ObjectPool<T> InternalPool { get; init; }

    private Pool(ObjectPool<T> internalPool) {
        this.InternalPool = internalPool;
    }
    
    public T Get() {
        T t = this.InternalPool.Get();
        t.OnReturn += this.Release;
        t.gameObject.SetActive(true);
        return t;
    }
    
    public PooledObject<T> Get(out T v) {
        PooledObject<T> obj = this.InternalPool.Get(out v);
        v.OnReturn += this.Release;
        v.gameObject.SetActive(true);
        return obj;
    }
    
    public void Release(T element) {
        this.InternalPool.Release(element);
        element.OnReturn -= this.Release;
        element.gameObject.SetActive(false);
    }
    
    public void Clear() {
        this.InternalPool.Clear();
    }
    
    public int CountInactive => this.InternalPool.CountInactive;

    public sealed class Builder {
        private Func<T> CreateFunc { get; set; }
        private Action<T>? ActionOnGet { get; set; }
        private Action<T>? ActionOnRelease { get; set; }
        private Action<T>? ActionOnDestroy { get; set; }
        private int DefaultCapacity { get; set; } = 10;
        private int MaxSize { get; set; } = 10000;
        
        private Builder(Func<T> creator) {
            this.CreateFunc = creator;
        }
    
        public static Builder Of(T seed) {
            return new Builder(() => Object.Instantiate(seed));
        }
        
        public Builder WithCapacity(int min, int max) {
            this.DefaultCapacity = min;
            this.MaxSize = max;
            return this;
        }
    
        public Builder WhenGet(Action<T> actionOnGet) {
            this.ActionOnGet += actionOnGet;
            return this;
        }

        public Builder WhenRelease(Action<T> actionOnRelease) {
            this.ActionOnRelease += actionOnRelease;
            return this;
        }
        
        public Builder WhenDestroy(Action<T> actionOnDestroy) {
            this.ActionOnDestroy += actionOnDestroy;
            return this;       
        }
        
        public Pool<T> Build() {
            return new Pool<T>(new ObjectPool<T>(this.CreateFunc, this.ActionOnGet, this.ActionOnRelease,
                this.ActionOnDestroy, true, this.DefaultCapacity, this.MaxSize));
        }
    }
}
