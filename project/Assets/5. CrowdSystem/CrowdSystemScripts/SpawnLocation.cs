using System;
using UnityEditor;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public float spawnRadius = 1;

//    public enum SpawnType
//    {
//        staticSpawn, 
//        randomSpawn, 
//        pingPongSpawn
//        
//    }

public BehaviourType CurrentSpawn;
    
    
//    public int GetSpawnPopulation(int maxPopulation, int factor)
//    {
//        var pop = Mathf.RoundToInt(spawnRadius * 20 * factor/ maxPopulation);
//        return pop;
//    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        switch (CurrentSpawn)
        {
            case BehaviourType.Static:
                Handles.color = Color.green;
                Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);

                break;
            case BehaviourType.Random:
                Handles.color = Color.red;
                Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);

                break;
            case BehaviourType.ForwardAndBack:
                Handles.color = Color.red;
                Handles.DrawLine(transform.position, transform.position + -transform.forward * 10);
                break;
            case BehaviourType.CrowdUp:
                break;
            case BehaviourType.Sitting:
                Handles.color = Color.gray;
                Handles.DrawWireDisc(transform.position , transform.up, spawnRadius);
                Debug.DrawLine(transform.position, transform.position + transform.forward * spawnRadius * 2, Color.blue);

                break;
            case BehaviourType.Talking:
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
