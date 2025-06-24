using System.Collections.Generic;
using UnityEngine;

namespace Flexalon.Runtime.Interaction
{
    internal class FlexalonRaycaster
    {
        public Vector3 hitPosition;

        private int _raycastFrame = 0;
        private FlexalonInteractable _hitInteractable;
        private readonly Dictionary<GameObject, FlexalonInteractable> _handles = new Dictionary<GameObject, FlexalonInteractable>();

#if UNITY_UI
        private List<UnityEngine.EventSystems.RaycastResult> _graphicRaycastResult = new List<UnityEngine.EventSystems.RaycastResult>();
#endif

#if UNITY_PHYSICS
        private RaycastHit[] _raycastHits = new RaycastHit[10];
#endif

        public void Register(FlexalonInteractable interactable)
        {
            this._handles.Add(interactable.Handle, interactable);
        }

        public void Unregister(FlexalonInteractable interactable)
        {
            this._handles.Remove(interactable.Handle);
        }

        public bool IsHit(Vector3 uiPointer, Ray ray, FlexalonInteractable interactable)
        {
            // Check if we've already casted this frame.
            if (this._raycastFrame != Time.frameCount)
            {
                this._hitInteractable = null;
                this._raycastFrame = Time.frameCount;
                float minDistance = float.MaxValue;
                this.RaycastUI(uiPointer, ref minDistance);
                this.RaycastPhysics(ray, ref minDistance);
            }

            return this._hitInteractable == interactable;
        }

        private void RaycastUI(Vector3 uiPointer, ref float minDistance)
        {
#if UNITY_UI
                var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                if (eventSystem)
                {
                    eventSystem.RaycastAll(new UnityEngine.EventSystems.PointerEventData(eventSystem)
                    {
                        position = uiPointer
                    }, this._graphicRaycastResult);

                    for (int i = 0; i < this._graphicRaycastResult.Count; i++)
                    {
                        var hit = this._graphicRaycastResult[i];
                        if (hit.distance < minDistance)
                        {
                            if (this._handles.TryGetValue(hit.gameObject, out var hitInteractable))
                            {
                                this._hitInteractable = hitInteractable;
                                minDistance = hit.distance;

                                hitInteractable.UpdateCanvas();

                                if (hitInteractable.Canvas?.renderMode == UnityEngine.RenderMode.ScreenSpaceOverlay)
                                {
                                    this.hitPosition = hit.screenPosition;
                                }
                                else
                                {
                                    this.hitPosition = hit.worldPosition;
                                }
                            }
                        }
                    }
                }
#endif
        }

        private void RaycastPhysics(Ray ray, ref float minDistance)
        {
#if UNITY_PHYSICS
            int hits = Physics.RaycastNonAlloc(ray, this._raycastHits, 1000);

            // Find the nearest hit interactable.
            for (int i = 0; i < hits; i++)
            {
                var hit = this._raycastHits[i];
                if (hit.distance < minDistance && this._handles.TryGetValue(hit.collider.gameObject, out var hitInteractable))
                {
                    this._hitInteractable = hitInteractable;
                    minDistance = hit.distance;
                    this.hitPosition = hit.point;
                }
            }
#endif
        }
    }
}