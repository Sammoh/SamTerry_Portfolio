using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "SittingBehaviour", menuName = "ScriptableObjects/SittingBehaviour", order = 3)]
    public class SittingBehaviour : BehaviourBase
    {
        public override void InitBehaviour(Vector3[] newWaypoints)
        {
            Debug.LogError("Just sitting.. maybe looking for another thing to do?");
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            Debug.LogError("No Destination");
            return Vector3.zero;
        }
    }
}