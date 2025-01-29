using Sammoh.Four;
using UnityEngine;

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
