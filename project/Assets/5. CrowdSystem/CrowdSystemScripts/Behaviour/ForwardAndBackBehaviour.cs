using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [CreateAssetMenu(fileName = "ForwardAndBackBehaviour", menuName = "ScriptableObjects/ForwardAndBackBehaviour", order = 3)]
    public class ForwardAndBackBehaviour : BehaviourBase
    {
        public override void InitBehaviour(Vector3[] newWaypoints)
        {
            Waypoints = new Vector3[2];
            var position = AgentAi.transform.position;
            Waypoints[0] = WalkingDistance * AgentAi.transform.forward + position;
            Waypoints[1] = position;
        }

        public override Vector3 GetBehaviourDestination(int step)
        {
            var direction = Waypoints[step % Waypoints.Length];
            return AgentAi.gameObject.transform.forward + direction;
        }
    }
}