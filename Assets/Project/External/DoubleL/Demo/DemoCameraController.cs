using UnityEngine;

namespace Project.External.DoubleL.Demo
{
    public class DemoCameraController : MonoBehaviour
    {
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                this.pitch = t.eulerAngles.x;
                this.yaw = t.eulerAngles.y;
                this.roll = t.eulerAngles.z;
                this.x = t.position.x;
                this.y = t.position.y;
                this.z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(this.pitch, this.yaw, this.roll) * translation;

                this.x += rotatedTranslation.x;
                this.y += rotatedTranslation.y;
                this.z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
            {
                this.yaw = Mathf.Lerp(this.yaw, target.yaw, rotationLerpPct);
                this.pitch = Mathf.Lerp(this.pitch, target.pitch, rotationLerpPct);
                this.roll = Mathf.Lerp(this.roll, target.roll, rotationLerpPct);

                this.x = Mathf.Lerp(this.x, target.x, positionLerpPct);
                this.y = Mathf.Lerp(this.y, target.y, positionLerpPct);
                this.z = Mathf.Lerp(this.z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(this.pitch, this.yaw, this.roll);
                t.position = new Vector3(this.x, this.y, this.z);
            }
        }

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        public float boost = 3.5f;
                
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
                
        public float rotationLerpTime = 0.01f;

        public bool invertY = false;

        void OnEnable()
        {
            this.m_TargetCameraState.SetFromTransform(this.transform);
            this.m_InterpolatingCameraState.SetFromTransform(this.transform);
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            return direction;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (this.invertY ? 1 : -1));

                var mouseSensitivityFactor = this.mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                this.m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                this.m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            
            var translation = this.GetInputTranslationDirection() * Time.deltaTime;
                        
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= 10.0f;
            }

            this.boost += Input.mouseScrollDelta.y * 0.2f;
            translation *= Mathf.Pow(2.0f, this.boost);

            this.m_TargetCameraState.Translate(translation);

            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / this.positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / this.rotationLerpTime) * Time.deltaTime);
            this.m_InterpolatingCameraState.LerpTowards(this.m_TargetCameraState, positionLerpPct, rotationLerpPct);

            this.m_InterpolatingCameraState.UpdateTransform(this.transform);
        }
    }
}
