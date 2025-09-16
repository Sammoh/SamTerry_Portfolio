using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject implementation of eating action
    /// </summary>
    [CreateAssetMenu(fileName = "EatAction", menuName = "GOAP/Actions/Eat Action", order = 1)]
    public class EatActionSO : NeedReductionActionSO
    {
        protected override void OnValidate()
        {
            // Set default values for eating
            actionType = "eat";
            cost = 1f;
            duration = 2f;
            needType = "hunger";
            targetNeedValue = 0f;
            requiredWorldFact = "at_food";
            needWorldStateKey = "need_hunger";
            
            base.OnValidate();
        }
        
        protected override void OnStartExecution(IAgentState agentState, IWorldState worldState)
        {
            Debug.Log("Started eating delicious food");
        }
        
        protected override void OnCancelExecution()
        {
            Debug.Log("Stopped eating - still hungry!");
        }
    }
}