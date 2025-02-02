using System;
using UnityEngine;

namespace Sammoh.CrowdSystem
{
    [Serializable, CreateAssetMenu(fileName = "NewCharacterDesign", menuName = "ScriptableObjects/NewCharacterDesign", order = 1)]
    public class CharacterDesigns: ScriptableObject
    {
        // old
        public CrowdAgentAi instancedCharacter;
        public Material instanceMaterial;
        
        public GameObject[] characterHeads;
        public GameObject[] characterTorsos;
        public GameObject[] characterLegs;
        
        
        public Texture2D[] characterSkin;
        public Texture2D[] characterColorVariants;
        // public CharacterPartBase[] CharacterHeads;
        // public CharacterPartBase[] CharacterTorso;
        // public CharacterPartBase[] CharacterLeg;

    }
}
