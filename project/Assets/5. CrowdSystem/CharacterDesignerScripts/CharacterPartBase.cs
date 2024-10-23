using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "NewCharacterPart", menuName = "ScriptableObjects/NewCharacterPart", order = 2)]
public class CharacterPartBase : ScriptableObject
{
//    public Mesh MeshComponents;
    public ColorScheme ColorScheme;
    public PartType PartType;
    public Material MaterialComponents;    
}
