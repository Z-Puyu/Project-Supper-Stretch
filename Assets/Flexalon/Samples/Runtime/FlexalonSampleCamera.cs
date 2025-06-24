using UnityEngine;

namespace Flexalon.Samples.Runtime
{
    // Simple camera controller.
    // Use WASD or arrows to move. Rotate with right mouse button.
    // Pan with mouse wheel button.
    public class FlexalonSampleCamera : MonoBehaviour
    {
        public float Speed = 0.2f;
        public float RotateSpeed = 0.2f;
        public float InterpolationSpeed = 20.0f;

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _alpha;
        private float _beta;
        private Vector3 _mousePos;

        void Start()
        {
            this._targetPosition = this.transform.position;
            this._targetRotation = this.transform.rotation;
            var euler = this._targetRotation.eulerAngles;
            this._alpha = euler.y;
            this._beta = euler.x;
        }

        void Update()
        {
#if UNITY_GUI
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            {
                return;
            }
#endif

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                this._targetPosition += this.transform.forward * this.Speed;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                this._targetPosition += -this.transform.right * this.Speed;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                this._targetPosition += this.transform.right * this.Speed;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                this._targetPosition += -this.transform.forward * this.Speed;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                this._mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var delta = Input.mousePosition - this._mousePos;
                this._alpha += delta.x * this.RotateSpeed;
                this._beta -= delta.y * this.RotateSpeed;
                this._targetRotation = Quaternion.Euler(this._beta, this._alpha, 0);
                this._mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonDown(2))
            {
                this._mousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                var delta = Input.mousePosition - this._mousePos;
                this._targetPosition -= delta.y * this.transform.up * this.Speed;
                this._targetPosition -= delta.x * this.transform.right * this.Speed;
                this._mousePos = Input.mousePosition;
            }

            this.transform.position = Vector3.Lerp(this.transform.position, this._targetPosition, Time.deltaTime * this.InterpolationSpeed);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this._targetRotation, Time.deltaTime * this.InterpolationSpeed);
        }
    }
}