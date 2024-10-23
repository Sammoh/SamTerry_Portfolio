using UnityEngine;

[CreateAssetMenu(fileName = "Adjectives", menuName = "GlobalData/Adjectives", order = 1)]
public class Adjectives : ScriptableObject
{
    public string[] adjectives = new string[10];
}