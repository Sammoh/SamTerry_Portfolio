using System;
using UnityEngine;

[Serializable]
public class AgentParts
{
    [SerializeField] GameObject[] characterHeads;
    public GameObject[] CharacterHeads => characterHeads;
    [SerializeField] GameObject[] characterTorsos;
    public GameObject[] CharacterTorsos => characterTorsos;
    [SerializeField] GameObject[] characterLegs;
    public GameObject[] CharacterLegs => characterLegs;

}
