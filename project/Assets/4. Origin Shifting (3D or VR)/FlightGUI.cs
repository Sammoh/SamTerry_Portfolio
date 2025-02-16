using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.Four
{
    public class FlightGUI : MonoBehaviour
    {
        // public PlaneController planeController;
        // public PlaneController otherPlaneController;

        // this object can be rotated.
        [SerializeField] private Image mapHud;

        [SerializeField] private Image radarPip; // this item will spawn for each of the characters available.

        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text PositionText;
        [SerializeField] private TMP_Text ShiftedPositionText;

        private void Awake()
        {
            // var plane = PlaneController.Instance.PlayerObject;
            // plane.GetFlightData += FlightUpdate;
            
            // find all the planes in the scene.
            var allPlanes = FindObjectsOfType<PlaneController>();
        }

        public void FlightUpdate(float speed, Vector3 position, Vector3 shiftedPosition)
        {
            // speedText.text = speed.ToString();
            // PositionText.text = position.ToString();
            // ShiftedPositionText.text = shiftedPosition.ToString();

            // GUI.Label(new Rect(10, 10, 200, 20), "Speed: " + planeController.GetCurrentSpeed());
            // GUI.Label(new Rect(10, 30, 200, 20), "Position: " + planeController.transform.position);
            // GUI.Label(new Rect(10, 50, 200, 20),
            //     "Shifted Position: " + GetShiftedPosition(planeController.transform.position));
            //
            // if (otherPlaneController != null)
            // {
            //     GUI.Label(new Rect(10, 70, 200, 20), "Other Plane Speed: " + otherPlaneController.GetCurrentSpeed());
            //     GUI.Label(new Rect(10, 90, 200, 20),
            //         "Other Plane Position: " + otherPlaneController.transform.position);
            // }
        }

        Vector3 GetShiftedPosition(Vector3 originalPosition)
        {
            // Implement your origin shifting logic here
            return originalPosition;
        }
    }
}