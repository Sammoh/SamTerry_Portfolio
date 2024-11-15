using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "RandomBehaviour", menuName = "ScriptableObjects/RandomBehaviour", order = 3)]
    public class RandomBehaviour : BehaviourBase
    {
        public override void InitBehaviour(Vector3[] newWaypoints)
        {
            Debug.LogError("Doing Something Random");
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            Debug.LogError("No Destination");
            return Vector3.zero;
        }
    }
}