using System;
using UnityEngine;

[Serializable]
public struct AgentParts
{
    public GameObject[] CharacterHeads => characterHeads;
    public GameObject[] CharacterTorsos => characterTorsos;
    public GameObject[] CharacterLegs => characterLegs;

    [SerializeField] GameObject[] characterHeads;
    [SerializeField] GameObject[] characterTorsos;
    [SerializeField] GameObject[] characterLegs;
    
    // from index 0 to 2 are female, add hair for male and female characters.
    public GameObject[] CharacterHairFemale => characterHairFemale;
    [SerializeField] GameObject[] characterHairFemale;
    public GameObject[] CharacterHairMale => characterHairMale;
    [SerializeField] GameObject[] characterHairMale;
    public GameObject[] CharacterHats => characterHats;
    [SerializeField] GameObject[] characterHats;
    
    // this is used to determine if the character is male or female.
    public bool IsFemale(int index)
    {
        return index < 3; // Assuming first three indices
    }


    public GameObject AddHair(int randomValue)
    {
        var hair = IsFemale(randomValue) ? CharacterHairFemale[UnityEngine.Random.Range(0, CharacterHairFemale.Length)] 
                                          : CharacterHairMale[UnityEngine.Random.Range(0, CharacterHairMale.Length)];
        var hairProbability = UnityEngine.Random.Range(0f, 1f);
        if (hairProbability < 0.1f) // 10% chance to not have hair
        {
            hair.SetActive(false);
            return hair;
        }
        
        hair.SetActive(true);
        return hair;
    }
}
