using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// ScriptableObject action for drinking at a water POI.
    /// Reduces thirst need to zero when at water location.
    /// </summary>
    [CreateAssetMenu(fileName = "DrinkAction", menuName = "GOAP/Actions/Drink", order = 11)]
    public class DrinkActionSO : NeedReductionActionSO
    {
        private void OnValidate()
        {
            // Configure default values for drinking
            actionType = "drink";
            requiredLocationKey = "at_water";
            needKey = "thirst";
            worldStateNeedKey = "need_thirst";
            targetNeedValue = 0f;
            duration = 1.5f;
            cost = 1f;
            debugMessage = "Started drinking";
        }
    }
}