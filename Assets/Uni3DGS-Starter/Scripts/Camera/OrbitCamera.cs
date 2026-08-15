using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class OrbitCamera : MonoBehaviour
    {
        public Transform Target;
        public float Distance = 5f;
        public float OrbitSpeed = 120f;
        public float ZoomSpeed = 4f;

        float yaw = 25f;
        float pitch = 15f;

        void LateUpdate()
        {
            if (Target == null) return;

            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * OrbitSpeed * Time.unscaledDeltaTime;
                pitch -= Input.GetAxis("Mouse Y") * OrbitSpeed * Time.unscaledDeltaTime;
                pitch = Mathf.Clamp(pitch, -85f, 85f);
            }

            Distance = Mathf.Clamp(Distance - Input.mouseScrollDelta.y * ZoomSpeed * 0.25f, 0.5f, 100f);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            transform.SetPositionAndRotation(Target.position - rotation * Vector3.forward * Distance, rotation);
        }

        public void ResetView()
        {
            yaw = 25f;
            pitch = 15f;
            Distance = 5f;
        }
    }
}
