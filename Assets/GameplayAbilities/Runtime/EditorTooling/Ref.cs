using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameplayAbilities.Runtime.EditorTooling { 
    [Serializable]
    public record struct Ref<I, T> where I : class where T : I {
        [SerializeField] private T? value = default;

        public I? Value => this.value;

        public T? RuntimeValue {
            get => this.value ?? default;
            set => this.value = value;
        }

        public Ref(T value) {
            this.value = value;
        }

        public Ref(I value) {
            this.value = value is T t ? t : default;
        }
        
        public static implicit operator Ref<I, T>(T obj) => new Ref<I, T>(obj);
        public static implicit operator I?(Ref<I, T> @ref) => @ref.Value;
        public static implicit operator T?(Ref<I, T> @ref) => @ref.RuntimeValue;
    }
    
    [Serializable]
    public record struct Ref<I> where I : class {
        [SerializeField] private Object? value;
        
        public I? Value => this.value as I;
        
        public Ref(Object value) {
            this.value = value;
        }
        
        public static implicit operator Ref<I>(Object obj) => new Ref<I>(obj);
        public static implicit operator I?(Ref<I> @ref) => @ref.Value;
    }
}
