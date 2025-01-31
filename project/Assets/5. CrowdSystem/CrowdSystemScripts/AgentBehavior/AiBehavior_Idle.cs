using Sammoh.CrowdSystem;
using UnityEngine;

public class AiBehavior_Idle : AIBehavior
{
    private static readonly int State = Animator.StringToHash("IdleState");

    [SerializeField] private float waitTime = 3f; // Editable in Inspector
    
    
    public AiBehavior_Idle(float waitTime)
    {
        this.waitTime = waitTime;
        // behavior should be the name off the class with a number
        // ChangeName($"Idle_{index}");
    }

    // public override void Execute()
    // {
    //     Debug.Log($"Executing Idle Behavior for {waitTime} seconds");
    //     
    //     // add an amount of random interval to the wait time
    //     waitTime += Random.Range(0, 3);
    //     // change the animation state to another idle state
    //     // inform that the job is done.
    //     // NewIdleState = Random.Range(0, 3);
    //     // wait for the time to pass
    //     // inform that the job is done.
    //     
    //     
    // }
    public override void Control(CrowdAgentAi crowdAgentAi)
    {
        // throw new NotImplementedException();
        // crowdAgentAi.SetAnimatorState(State);
        // crowdAgentAi.Wait(waitTime);
    }
}