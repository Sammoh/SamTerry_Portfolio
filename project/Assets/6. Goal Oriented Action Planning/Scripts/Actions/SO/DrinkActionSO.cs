using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject implementation of drinking action
    /// </summary>
    [CreateAssetMenu(fileName = "DrinkAction", menuName = "GOAP/Actions/Drink Action", order = 2)]
    public class DrinkActionSO : NeedReductionActionSO
    {
        private void Awake()
        {
            // Set default values for drinking
            actionType = "drink";
            cost = 1f;
            duration = 1.5f;
            needType = "thirst";
            targetNeedValue = 0f;
            requiredWorldFact = "at_water";
            needWorldStateKey = "need_thirst";
        }
        
        protected override void OnStartExecution(IAgentState agentState, IWorldState worldState)
        {
            Debug.Log("Started drinking refreshing water");
        }
        
        protected override void OnCancelExecution()
        {
            Debug.Log("Stopped drinking - still thirsty!");
        }
    }
}