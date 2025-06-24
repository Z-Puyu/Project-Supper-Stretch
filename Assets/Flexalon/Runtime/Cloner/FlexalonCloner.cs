using System.Collections.Generic;
using UnityEngine;

namespace Flexalon.Runtime.Cloner
{
    /// <summary>
    /// Sometimes, it's useful to generate child objects instead of defining them statically.
    /// The Flexalon Cloner can generate objects from a set of prefabs iteratively or randomly,
    /// and can optionally bind to a data source.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Cloner"), HelpURL("https://www.flexalon.com/docs/cloner")]
    public class FlexalonCloner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _objects;
        /// <summary> Prefabs which should be cloned as children. </summary>
        public List<GameObject> Objects
        {
            get => this._objects;
            set { this._objects = value; this.MarkDirty(); }
        }

        /// <summary> In which order should prefabs be cloned. </summary>
        public enum CloneTypes
        {
            /// <summary> Clone prefabs in the order they are assigned. </summary>
            Iterative,

            /// <summary> Clone prefabs in a random order. </summary>
            Random
        }

        [SerializeField]
        private CloneTypes _cloneType = CloneTypes.Iterative;
        /// <summary> In which order should prefabs be cloned. </summary>
        public CloneTypes CloneType
        {
            get => this._cloneType;
            set { this._cloneType = value; this.MarkDirty(); }
        }

        [SerializeField]
        private uint _count;
        /// <summary> How many clones should be generated. </summary>
        public uint Count
        {
            get => this._count;
            set { this._count = value; this.MarkDirty(); }
        }

        [SerializeField]
        private int _randomSeed;
        /// <summary> Seed used for the Random clone type, to ensure results remain consistent. </summary>
        public int RandomSeed
        {
                get => this._randomSeed;
                set { this._randomSeed = value; this.MarkDirty(); }
        }

        [SerializeField]
        private GameObject _dataSource = null;
        /// <summary> Can be an gameObject with a component that implements FlexalonDataSource.
        /// The number of objects cloned is set to the number of items in the Data property. </summary>
        public GameObject DataSource
        {
            get => this._dataSource;
            set
            {
                this.UnhookDataSource();
                this._dataSource = value;
                this.HookDataSource();
                this.MarkDirty();
            }
        }

        [SerializeField, HideInInspector]
        private List<GameObject> _clones = new List<GameObject>();

        void OnEnable()
        {
            this.HookDataSource();
            this.MarkDirty();
        }

        private void HookDataSource()
        {
            if (this.isActiveAndEnabled && this._dataSource != null && this._dataSource)
            {
                if (this._dataSource.TryGetComponent<DataSource>(out var component))
                {
                    component.DataChanged += this.MarkDirty;
                }
            }
        }

        private void UnhookDataSource()
        {
            if (this._dataSource != null && this._dataSource)
            {
                if (this._dataSource.TryGetComponent<DataSource>(out var component))
                {
                    component.DataChanged -= this.MarkDirty;
                }
            }
        }

        void OnDisable()
        {
            this.UnhookDataSource();
            this.MarkDirty();
        }

        /// <summary> Forces the cloner to regenerate its clones. </summary>
        public void MarkDirty()
        {
            foreach(var clone in this._clones)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(clone);
                }
                else
                {
                    Object.DestroyImmediate(clone);
                }
            }

            this._clones.Clear();

            if (this.isActiveAndEnabled && this._objects != null && this._objects.Count > 0)
            {
                switch (this._cloneType)
                {
                    case CloneTypes.Iterative:
                        this.GenerateIterativeClones();
                        break;
                    case CloneTypes.Random:
                        this.GenerateRandomClones();
                        break;
                }
            }
        }

        private IReadOnlyList<object> GetData()
        {
            if (this._dataSource != null && this._dataSource)
            {
                return this._dataSource.GetComponent<DataSource>()?.Data;
            }

            return null;
        }

        private void GenerateIterativeClones()
        {
            int i = 0;
            var data = this.GetData();
            var count = data?.Count ?? (int)this._count;
            while (this._clones.Count < count)
            {
                this.GenerateClone(i, data);
                i = (i + 1) % this._objects.Count;
            }
        }

        private void GenerateRandomClones()
        {
            var random = new System.Random(this._randomSeed);
            var data = this.GetData();
            var count = data?.Count ?? (int)this._count;
            while (this._clones.Count < count)
            {
                this.GenerateClone(random.Next(this._objects.Count), data);
            }
        }

        private void GenerateClone(int index, IReadOnlyList<object> data)
        {
            var clone = Object.Instantiate(this._objects[index], Vector3.zero, Quaternion.identity, this.transform);
            this._clones.Add(clone);

            if (data != null && clone.TryGetComponent<DataBinding>(out var dataBinding))
            {
                dataBinding.SetData(data[this._clones.Count - 1]);
            }
        }
    }
}