using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "NewCharacterDesign", menuName = "ScriptableObjects/NewCharacterDesign", order = 1)]
public class CharacterDesigns: ScriptableObject
{
    public GameObject instancedCharacter;
    public CharacterPartBase[] CharacterHeads;
    public CharacterPartBase[] CharacterTorso;
    public CharacterPartBase[] CharacterLeg;

}
