using System;
using CommonFrameworks.Utilities;
using SaintsField;
using SaveAndLoad.Momentos;
using UnityEngine;

namespace SaveAndLoad {
    [DisallowMultipleComponent]
    public abstract class SaveDataProcessor<T, S> : MonoBehaviour, ISaveable
            where T : Component where S : IMomento<T>, new() {
        [field: SerializeField, ReadOnly] private string Id { get; set; } = Guid.NewGuid().ToString();

        [field: SerializeField, Required] private T? Component { get; set; }
        private S? Data { get; set; }

        private void Awake() {
            if (!this.Component) {
                this.Component = this.GetComponent<T>();
            }

            this.Load();
        }

        private void Load() {
            if (!this.Component) {
                Debug.LogWarning($"Data for component {typeof(T)} will not be saved nor loaded.", this);
            } else {
                this.Data = Singleton<SaveGameSystem>.Instance.ReadSaveData<S>(this.Id);
                this.Data.Restore(this.Component);
            }
        }

        void ISaveable.Save() {
            if (!this.Component || this.Data is null) {
                return;
            }

            this.Data.Capture(this.Component);
            Singleton<SaveGameSystem>.Instance.WriteSaveData(this.Id, this.Data);
        }

        void ISaveable.Load() {
            this.Load();
        }
    }
}
