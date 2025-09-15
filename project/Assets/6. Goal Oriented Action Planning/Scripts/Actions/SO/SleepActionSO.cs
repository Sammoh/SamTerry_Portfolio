using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject action for sleeping at a bed/rest location.
    /// Reduces sleep need to zero when at bed location.
    /// </summary>
    [CreateAssetMenu(fileName = "SleepAction", menuName = "GOAP/Actions/Sleep", order = 13)]
    public class SleepActionSO : NeedReductionActionSO
    {
        private void OnValidate()
        {
            // Configure default values for sleeping
            actionType = "sleep";
            requiredLocationKey = "at_bed";
            needKey = "sleep";
            worldStateNeedKey = "need_sleep";
            targetNeedValue = 0f;
            duration = 3f;
            cost = 1f;
            debugMessage = "Started sleeping";
        }
    }
}