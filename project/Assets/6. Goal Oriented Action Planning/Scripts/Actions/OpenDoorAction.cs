using System.Collections.Generic;

namespace Sammoh.GOAP
{
    public class OpenDoorAction : IAction
    {
        public string ActionType { get; }
        public float Cost { get; }
        
        
        public bool CheckPreconditions(IAgentState agentState, IWorldState worldState)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<string, object> GetEffects()
        {
            throw new System.NotImplementedException();
        }

        public void StartExecution(IAgentState agentState, IWorldState worldState)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult UpdateExecution(IAgentState agentState, IWorldState worldState, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void CancelExecution()
        {
            throw new System.NotImplementedException();
        }

        public void ApplyEffects(IAgentState agentState, IWorldState worldState)
        {
            throw new System.NotImplementedException();
        }

        public bool IsExecuting { get; }
        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}