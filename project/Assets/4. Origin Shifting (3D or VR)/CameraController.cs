using UnityEngine;

namespace Sammoh.Four
{
    public class CameraController : MonoBehaviour
    {
        private PlaneController _currentPlane;
        public Camera mainCamera;
        public float followSpeed = 5f;
        public Vector3 offset = new Vector3(0, 10, -20); // Offset for the camera

        public void SwitchToPlane(PlaneController newPlane)
        {
            _currentPlane = newPlane;
        }

        private void UpdateCameraPosition()
        {
            Vector3 targetPosition = _currentPlane.transform.position + offset;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition,
                followSpeed * Time.deltaTime);
            mainCamera.transform.LookAt(_currentPlane.transform);
        }

        private void LateUpdate()
        {
            if (_currentPlane != null)
            {
                UpdateCameraPosition();
            }
        }
    }
}