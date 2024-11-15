using System;
using UnityEditor;
using UnityEngine;

namespace Sammoh.CrowdSystem
{
    public class SpawnLocation : MonoBehaviour
    {
        public float spawnRadius = 1;
        public BehaviourBase spawnBehaviour;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (spawnBehaviour == null) return;
            
            switch (spawnBehaviour)
            {
                case StaticBehaviour _:
                    Handles.color = Color.green;
                    Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);
                    break;
                case RandomBehaviour _:
                    Handles.color = Color.red;
                    Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);
                    break;
                case ForwardAndBackBehaviour _:
                    Handles.color = Color.red;
                    Handles.DrawLine(transform.position, transform.position + -transform.forward * 10);
                    break;
                case CrowdingBehaviour _:
                    break;
                case SittingBehaviour _:
                    Handles.color = Color.gray;
                    Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);
                    break;
                case TalkingBehaviour _:
                    Handles.color = Color.yellow;
                    Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                    Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif

    }
}
