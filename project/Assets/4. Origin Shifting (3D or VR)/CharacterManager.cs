using UnityEngine;

namespace SpecularTheory.Four
{
    public class CharacterManager : MonoBehaviour
    {
        public OriginShifting originShifting; // Reference to the OriginShifting script

        public void ChangeCharacter(Transform newCharacter)
        {
            originShifting.SwitchCharacter(newCharacter);
        }
    }

}
