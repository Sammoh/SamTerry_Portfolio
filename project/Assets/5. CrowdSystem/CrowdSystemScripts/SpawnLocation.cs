using UnityEditor;
using UnityEngine;

namespace Sammoh.CrowdSystem
{
    public class SpawnLocation : MonoBehaviour
    {
        public float spawnRadius = 1;
        // public BehaviourBase spawnBehaviour;

        [Header("Tasks")]
        // [SerializeReference]
        // private List<AIBehavior> objectives = new List<AIBehavior>();
        [SerializeReference]
        private AIBehavior behaviour;
        public AIBehavior SpawnBehaviour => behaviour;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (behaviour == null) return;

            switch (behaviour)
            {
                case AiBehavior_Crowding aiBehaviorCrowding:
                    Handles.color = Color.black;
                    Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 1.5f, Color.blue);
                    break;
                case AiBehavior_Idle _:
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 1.5f, Color.blue);
                    break;
                case AiBehavior_Sitting aiBehaviorSitting:
                    Handles.color = Color.blue;
                    Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 1.5f, Color.blue);
                    break;
                case AiBehavior_Talking aiBehaviorTalking:
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 1.5f, Color.blue);
                    break;
                case AiBehavior_ForwardBack forwardBack:
                    Handles.color = Color.red;
                    var distance = forwardBack.travelDistance;
                    Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * distance, Color.blue);
                    break;
            }
        }
#endif

    }
}
