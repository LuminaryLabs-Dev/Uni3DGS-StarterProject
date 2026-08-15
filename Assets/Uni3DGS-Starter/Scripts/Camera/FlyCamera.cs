using UnityEngine;

namespace Uni3DGS.Starter
{
    public sealed class FlyCamera : MonoBehaviour
    {
        public float MoveSpeed = 4f;
        public float LookSpeed = 2f;

        void Update()
        {
            if (!Input.GetMouseButton(2)) return;
            float yaw = Input.GetAxis("Mouse X") * LookSpeed;
            float pitch = -Input.GetAxis("Mouse Y") * LookSpeed;
            transform.Rotate(Vector3.up, yaw, Space.World);
            transform.Rotate(Vector3.right, pitch, Space.Self);

            Vector3 move = new(
                Input.GetAxisRaw("Horizontal"),
                (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f),
                Input.GetAxisRaw("Vertical"));
            transform.Translate(move.normalized * (MoveSpeed * Time.unscaledDeltaTime), Space.Self);
        }
    }
}
