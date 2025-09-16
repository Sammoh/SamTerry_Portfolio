using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject implementation of sleeping action
    /// </summary>
    [CreateAssetMenu(fileName = "SleepAction", menuName = "GOAP/Actions/Sleep Action", order = 3)]
    public class SleepActionSO : NeedReductionActionSO
    {
        private void Awake()
        {
            // Set default values for sleeping
            actionType = "sleep";
            cost = 1f;
            duration = 3f;
            needType = "sleep";
            targetNeedValue = 0f;
            requiredWorldFact = "at_bed";
            needWorldStateKey = "need_sleep";
        }
        
        protected override void OnStartExecution(IAgentState agentState, IWorldState worldState)
        {
            Debug.Log("Started sleeping peacefully... Zzz");
        }
        
        protected override void OnCancelExecution()
        {
            Debug.Log("Woke up too early - still tired!");
        }
    }
}