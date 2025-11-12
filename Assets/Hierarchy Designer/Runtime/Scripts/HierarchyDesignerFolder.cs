using UnityEngine;
using UnityEngine.Events;

namespace Hierarchy_Designer.Runtime.Scripts
{
    public class HierarchyDesignerFolder : MonoBehaviour
    {
        #region Properties
        [Tooltip("Flatten Folder = Free all the folder's GameObject children in the Awake/Start method (FlattenEvent), then once the operation is complete, destroy the folder.")]
        [SerializeField] private bool flattenFolder = true;
        public bool ShouldFlatten => this.flattenFolder;

        public enum FlattenEvent { Awake, Start }
        [Tooltip("FlattenEvent.Awake = The Flatten Folder Operation will occur in the Awake method.\nFlattenEvent.Start = The Flatten Folder Operation will occur in the Start method.\n\n*Use FlattenEvent.Awake if you have gameObjects with Singleton patterns with DontDestroyOnLoad in the Start Method or similar.*")]
        [SerializeField] private FlattenEvent flattenEvent = FlattenEvent.Start;

        [Tooltip("Event(s) called just before the flatten event occurs.")] [SerializeField] private UnityEvent OnFlattenEvent;
        [Tooltip("Event(s) called just before the folder is destroyed.")] [SerializeField] private UnityEvent OnFolderDestroy;

        #if UNITY_EDITOR
        [HideInInspector] [SerializeField] private string notes;
        #endif

        private Transform cachedTransform;
        #endregion

        #region Initialization
        private void Awake()
        {
            this.cachedTransform = this.transform;
            this.HandleFlattenEvent(FlattenEvent.Awake);
        }

        private void Start()
        {
            this.HandleFlattenEvent(FlattenEvent.Start);
        }
        #endregion

        #region Operations
        private void HandleFlattenEvent(FlattenEvent eventToCheck)
        {
            if (this.flattenFolder && this.flattenEvent == eventToCheck)
            {
                this.OnFlattenEvent?.Invoke();
                this.FlattenFolderIfRequired();
            }
        }

        private void FlattenFolderIfRequired()
        {
            this.RecursivelyFlatten(this.cachedTransform);
            this.OnFolderDestroy?.Invoke();
            Object.Destroy(this.gameObject);
        }

        private void RecursivelyFlatten(Transform folderTransform)
        {
            Transform[] childrenToDestroy = new Transform[folderTransform.childCount];
            int destroyIndex = 0;

            for (int i = folderTransform.childCount - 1; i >= 0; i--)
            {
                Transform childTransform = folderTransform.GetChild(i);
                HierarchyDesignerFolder childFolder = childTransform.GetComponent<HierarchyDesignerFolder>();

                if (childFolder != null && childFolder.ShouldFlatten)
                {
                    childFolder.RecursivelyFlatten(childTransform);
                    childrenToDestroy[destroyIndex++] = childTransform;
                }
                else
                {
                    childTransform.SetParent(null);
                }
            }

            for (int i = 0; i < destroyIndex; i++)
            {
                Object.Destroy(childrenToDestroy[i].gameObject);
            }
        }
        #endregion
    }
}