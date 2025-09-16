using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject implementation of playing action
    /// </summary>
    [CreateAssetMenu(fileName = "PlayAction", menuName = "GOAP/Actions/Play Action", order = 4)]
    public class PlayActionSO : NeedReductionActionSO
    {
        protected override void OnValidate()
        {
            // Set default values for playing
            actionType = "play";
            cost = 1f;
            duration = 2f;
            needType = "play";
            targetNeedValue = 0f;
            requiredWorldFact = "at_toy";
            needWorldStateKey = "need_play";
            
            base.OnValidate();
        }
        
        protected override void OnStartExecution(IAgentState agentState, IWorldState worldState)
        {
            Debug.Log("Started playing with toys - so much fun!");
        }
        
        protected override void OnCancelExecution()
        {
            Debug.Log("Stopped playing - want to play more!");
        }
    }
}