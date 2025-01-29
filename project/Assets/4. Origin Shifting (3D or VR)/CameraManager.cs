using UnityEngine;

namespace Sammoh.Four
{
    public class CameraManager : MonoBehaviour
    {
        public Transform cameraTransform;
        public float followSpeed = 5f;
        public float rotationSpeed = 5f;
        public Vector3 offset = new Vector3(0, 10, -20); // Offset for the camera

        private Transform target;

        void Start()
        {
            PlaneManager.Instance.OnControlSwitch += UpdateCameraTarget; // Listen to the control switch event
            UpdateCameraTarget(PlaneManager.Instance.PlayerPlane); // Directly set the initial camera target
        }

        void LateUpdate()
        {
            if (target == null) return;

            // Calculate the desired position behind the target
            Vector3 desiredPosition = target.position + target.forward * offset.z + Vector3.up * offset.y;

            // Smooth position follow
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Smooth rotation follow
            Quaternion targetRotation = Quaternion.LookRotation(target.position - cameraTransform.position);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void UpdateCameraTarget(PlaneController newPlaneName)
        {
            if (newPlaneName != null)
                target = newPlaneName.transform;
        }
    }
}