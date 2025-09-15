using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject action for eating at a food POI.
    /// Reduces hunger need to zero when at food location.
    /// </summary>
    [CreateAssetMenu(fileName = "EatAction", menuName = "GOAP/Actions/Eat", order = 10)]
    public class EatActionSO : NeedReductionActionSO
    {
        private void OnValidate()
        {
            // Configure default values for eating
            actionType = "eat";
            requiredLocationKey = "at_food";
            needKey = "hunger";
            worldStateNeedKey = "need_hunger";
            targetNeedValue = 0f;
            duration = 2f;
            cost = 1f;
            debugMessage = "Started eating";
        }
    }
}