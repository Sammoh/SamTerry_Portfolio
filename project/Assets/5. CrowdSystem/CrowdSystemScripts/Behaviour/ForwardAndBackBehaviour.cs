using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "ForwardAndBackBehaviour", menuName = "ScriptableObjects/ForwardAndBackBehaviour", order = 3)]
    public class ForwardAndBackBehaviour : BehaviourBase
    {
        public override void InitBehaviour(CrowdAgentAi agentAi, Vector3[] newWaypoints)
        {
            _AgentAi = agentAi;
            Waypoints = new Vector3[2];
            var position = agentAi.transform.position;
            // the forward direction of the agent.
            Waypoints[0] = position + agentAi.transform.forward;
            Waypoints[1] = position;
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            var direction = Waypoints[step % Waypoints.Length];
            return _AgentAi.gameObject.transform.forward + direction;
        }
    }
}