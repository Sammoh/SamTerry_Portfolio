using UnityEngine;

namespace SpecularTheory.Four
{
    public class GameController : MonoBehaviour
    {
        public CharacterManager characterManager;

        void Start()
        {
            // Example usage
            Transform newCharacter = GameObject.Find("NewCharacter").transform;
            characterManager.ChangeCharacter(newCharacter);
        }
    }
}