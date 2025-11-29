using UnityEngine;

namespace UnityTechnologies.ParticlePack.Shared.Scripts {
    public class ProximityActivate : MonoBehaviour
    {

        public Transform distanceActivator, lookAtActivator;
        public float distance;
        public Transform activator;
        public bool activeState = false;
        public CanvasGroup target;
        public bool lookAtCamera = true;
        public bool enableInfoPanel = false;
        public GameObject infoIcon;

        float alpha;
        public CanvasGroup infoPanel;
        Quaternion originRotation, targetRotation;

        void Start()
        {
            this.originRotation = this.transform.rotation;
            this.alpha = this.activeState ? 1 : -1;
            if (this.activator == null) this.activator = Camera.main.transform;
            this.infoIcon.SetActive(this.infoPanel != null);
        }

        bool IsTargetNear()
        {
            var distanceDelta = this.distanceActivator.position - this.activator.position;
            if (distanceDelta.sqrMagnitude < this.distance * this.distance)
            {
                if (this.lookAtActivator != null)
                {
                    var lookAtActivatorDelta = this.lookAtActivator.position - this.activator.position;
                    if (Vector3.Dot(this.activator.forward, lookAtActivatorDelta.normalized) > 0.95f)
                        return true;
                }
                var lookAtDelta = this.target.transform.position - this.activator.position;
                if (Vector3.Dot(this.activator.forward, lookAtDelta.normalized) > 0.95f)
                    return true;
            }
            return false;
        }

        void Update()
        {
            if (!this.activeState)
            {
                if (this.IsTargetNear())
                {
                    this.alpha = 1;
                    this.activeState = true;
                }
            }
            else
            {
                if (!this.IsTargetNear())
                {
                    this.alpha = -1;
                    this.activeState = false;
                    this.enableInfoPanel = false;
                }
            }
            this.target.alpha = Mathf.Clamp01(this.target.alpha + this.alpha * Time.deltaTime);
            if (this.infoPanel != null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    this.enableInfoPanel = !this.enableInfoPanel;
                this.infoPanel.alpha = Mathf.Lerp(this.infoPanel.alpha, Mathf.Clamp01(this.enableInfoPanel ? this.alpha : 0), Time.deltaTime * 10);
            }
            if (this.lookAtCamera)
            {
                if (this.activeState)
                    this.targetRotation = Quaternion.LookRotation(this.activator.position - this.transform.position);
                else
                    this.targetRotation = this.originRotation;
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.targetRotation, Time.deltaTime);
            }
        }

    }
}
