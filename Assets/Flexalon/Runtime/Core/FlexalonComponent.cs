using UnityEngine;
using UnityEngine.Serialization;

namespace Flexalon.Runtime.Core
{
    /// <summary>
    /// Base type for many Flexalon components. Deals with FlexalonNode lifecycle,
    /// and provides the ForceUpdate and MarkDirty methods to trigger a Flexalon update.
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(FlexalonResult))]
    public abstract class FlexalonComponent : MonoBehaviour
    {
        protected FlexalonNode _node;

        /// <summary> The FlexalonNode associated with this gameObject. </summary>
        public FlexalonNode Node => this._node;

        [SerializeField, HideInInspector, FormerlySerializedAs("_initialized")]
        private int _version;

        private static readonly int _currentVersion = 4;

        void Update()
        {
            this.DoUpdate();
        }

        void OnEnable()
        {
            this._node = Flexalon.GetOrCreateNode(this.gameObject);

            this.DoOnEnable();

            if (this._version == 0)
            {
                this.Initialize();
            }
            else if (this._version < FlexalonComponent._currentVersion)
            {
                this.Upgrade(this._version);
            }

            if (!this._node.HasResult || this._version == 0)
            {
                this.MarkDirty();
            }
            else
            {
                this.UpdateProperties();
            }

            this._version = FlexalonComponent._currentVersion;
        }

        void OnDisable()
        {
            this.DoOnDisable();
        }

        void OnDestroy()
        {
            if (this._node != null)
            {
                this.ResetProperties();
                Flexalon.RecordFrameChanges = true;
                this._node.MarkDirty();
                this._node = null;
            }
        }

        /// <summary> Marks this component needing an update. The Flexalon singleton
        /// will visit it in dependency order on LateUpdate. </summary>
        public void MarkDirty()
        {
            if (this._node != null)
            {
                this.UpdateProperties();
                this._node.MarkDirty();
            }
        }

        /// <summary> Forces this component, its parent nodes, and its children nodes to update immediately. </summary>
        public void ForceUpdate()
        {
            this._node = Flexalon.GetOrCreateNode(this.gameObject);
            this.MarkDirty();
            this._node.ForceUpdate();
        }

        void OnDidApplyAnimationProperties()
        {
            this.MarkDirty();
        }

        /// <summary> Called when the component is enabled to apply properties to the FlexalonNode. </summary>
        protected virtual void UpdateProperties() {}

        /// <summary> Called when the component is destroyed to reset properties on the FlexalonNode. </summary>
        protected virtual void ResetProperties() {}

        /// <summary> Called when the component is enabled. </summary>
        protected virtual void DoOnEnable() {}

        /// <summary> Called when the component is disabled. </summary>
        protected virtual void DoOnDisable() {}

        /// <summary> Called when the component is updated. </summary>
        public virtual void DoUpdate() {}

        /// <summary> Called when the component is first created. </summary>
        protected virtual void Initialize() {}

        /// <summary> Called when the component is upgraded to a new version of Flexalon. </summary>
        protected virtual void Upgrade(int fromVersion) {}
    }
}