using System;
using CrowdSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable, CreateAssetMenu(fileName = "NewAgentBehaviour", menuName = "ScriptableObjects/NewAgentBehaviour", order = 3)]
public class BehaviourBase : ScriptableObject
{
    public float PaceRangeMax;
    public float PaceRangeMin;

    public float radius;

    private bool firstBehaviour;

    public float walkingDistance = 10;



    // this could get a determined value of waypoints. 
    [FormerlySerializedAs("_waypoints")] public Vector3[] waypoints;
    [SerializeField] private Vector3 _currentDestination;

    public BehaviourType Behaviour;
        
    [HideInInspector]
    public CrowdAgentAi AgentAi;

    public void InitBehaviour()
    {
        
    }
    
    public Vector3[] InitBehaviour(Vector3[] newWaypoints, BehaviourType initType)
    {
        switch (initType)
        {
            case BehaviourType.Static:
                // do nothing. 
                break;
            case BehaviourType.Random:
                // do nothing. 
                break;
            case BehaviourType.ForwardAndBack:
                // this should find two waypoints. 
                waypoints = InitForwardBackWaypoints();
                break;
            case BehaviourType.CrowdUp:
                // this should do a poisson disk search on the nav mesh. 
                break;
            case BehaviourType.Sitting:
                // do nothing. 
                break;
            case BehaviourType.Talking:
                // do nothing. 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(initType), initType, null);
        }
//         _waypoints = newWaypoints;

         return waypoints;
    }
    
    public Vector3 GetBehaviourDestination(int step)
    {
        var d = new Vector3();
        
        switch (Behaviour)
        {
            case BehaviourType.Static:
                break;
            case BehaviourType.Random:
                d = RandomNavmeshLocation();
                break;
            case BehaviourType.ForwardAndBack:
//                d = ForwardAndBack(); // this should just go from waypoint to waypoint. 
                d = NextWaypoint(step);
                break;
            case BehaviourType.CrowdUp:
//                d = NewCrowdPosition(step, _waypoints);
                break;
            case BehaviourType.Sitting:
                break;
            case BehaviourType.Talking:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _currentDestination = d;
        return d;
    }

    //only returns what's behind the agent.
    private Vector3 ForwardAndBack(int step)
    {
        return -walkingDistance * AgentAi.transform.forward + AgentAi.transform.position;
    }

    private Vector3[] InitForwardBackWaypoints()
    {
        
        var transform = AgentAi.transform;
        var position = transform.position;

        return new [] {-walkingDistance * transform.forward + position, position};
    }

    private Vector3 DoubleForwardDoubleBack(int step)
    {
        //switch every other behaviour
        return step % 2 == 0
            ? AgentAi.transform.position + AgentAi.transform.forward
            : AgentAi.transform.position + -AgentAi.transform.forward;
    }

    private Vector3 NewCrowdPosition(int step, Vector3[] crowdPoints)
    {
        var p = Mathf.PerlinNoise(PaceRangeMin, step );
        
        
//        _waypoints = 
        //randomly within the radius guides, do either find new crowd or  stay in current crowd. 
        return step % radius == 0 ? FindNewCrowd() : AgentAi.transform.position;
    }

    // needs to have an initial crowd count. 
    private Vector3 FindNewCrowd()
    {
        var l = waypoints.Length;
        var v = AgentAi.transform.position + AgentAi.transform.forward * radius;
        // use a search radius to find the nearest.
        foreach (var t in waypoints)
        {
            if (!(Vector3.Distance(AgentAi.transform.position, t) <= radius)) continue;
            
//            Debug.LogError(new Vector3(t.x, t.y, t.z));
            v = t;
            break;
        }

        return v;
    }

    private Vector3 NextWaypoint(int step)
    {
        if (waypoints.Length == 0)
            throw new ArgumentException();

        return waypoints[step];
    }
    
    //returns a random point around an agent within a radius.
    private Vector3 RandomNavmeshLocation() {
        var randomDirection = Random.insideUnitSphere * radius;
        randomDirection += AgentAi.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    private Vector3 GetStaticPosition(int waypoint)
    {
        return waypoints[waypoint];
    }
}
