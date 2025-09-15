using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject action for playing with a toy at current location.
    /// Reduces play need to zero when at toy location.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayAction", menuName = "GOAP/Actions/Play", order = 12)]
    public class PlayActionSO : NeedReductionActionSO
    {
        private void OnValidate()
        {
            // Configure default values for playing
            actionType = "play";
            requiredLocationKey = "at_toy";
            needKey = "play";
            worldStateNeedKey = "need_play";
            targetNeedValue = 0f;
            duration = 2f;
            cost = 1f;
            debugMessage = "Started playing";
        }
    }
}