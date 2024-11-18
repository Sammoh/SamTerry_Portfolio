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
            PlaneManager.Instance.OnControlSwitch.AddListener(UpdateCameraTarget);
            UpdateCameraTarget(PlaneManager.Instance.PlayerPlane); // Directly set the initial camera target

        }

        void Update()
        {
            if (target == null) return;

            // Smooth position follow
            cameraTransform.position =
                Vector3.Lerp(cameraTransform.position, target.position + offset, followSpeed * Time.deltaTime);

            // Smooth rotation follow
            Quaternion targetRotation = Quaternion.LookRotation(target.forward);
            cameraTransform.rotation =
                Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            // Look at the target
            cameraTransform.LookAt(target);
        }

        private void UpdateCameraTarget(PlaneController newPlaneName)
        {
            if (newPlaneName != null)
                target = newPlaneName.transform;
        }
    }
}