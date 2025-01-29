using UnityEngine;

namespace Sammoh.Four
{

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private FloatingOrigin originShifting;
        [SerializeField] private Transform worldOrigin;

        private PlaneManager flightController;

        private void Start()
        {
            flightController = PlaneManager.Instance;
            flightController.OnControlSwitch += OnControlSwitch;

        }



        private void OnControlSwitch(PlaneController obj)
        {
        }

    }
}
