using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "StaticBehaviour", menuName = "ScriptableObjects/StaticBehaviour", order = 3)]
    public class StaticBehaviour : BehaviourBase
    {
        public override void InitBehaviour(Vector3[] newWaypoints)
        {
            Debug.LogError("Doing nothing");
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            Debug.LogError("No Destination");
            return Vector3.zero;
        }
    }
}
