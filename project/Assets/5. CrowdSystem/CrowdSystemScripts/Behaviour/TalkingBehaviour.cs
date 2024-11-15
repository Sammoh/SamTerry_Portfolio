using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "TalkingBehaviour", menuName = "ScriptableObjects/TalkingBehaviour", order = 3)]
    public class TalkingBehaviour : BehaviourBase
    {
        public override void InitBehaviour(CrowdAgentAi agentAi, Vector3[] newWaypoints)
        {
            Debug.LogError("Just talking.. maybe looking for another thing to do?");
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            Debug.LogError("No Destination");
            return Vector3.zero;
        }
    }
}