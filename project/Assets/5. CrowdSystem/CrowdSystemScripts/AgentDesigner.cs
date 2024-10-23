using CrowdSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentDesigner
{
    private const int SchemeOffset = 3;
    private const int MaxSeedValue = 3;

    public AgentDesigner()
    {
    }

    private Animator _animator;

    int schemeIndex = 0;
    int headCount = 0;
    int topCount = 0;
    int botCount = 0;

    public void AssignRandomCharacter(CrowdAgentAi agentAi, CharacterDesigns characterDesign)
    {
        headCount = 0;
        topCount = 0;
        botCount = 0;

        var randScheme = (ColorScheme)Random.Range(0, MaxSeedValue);
        var headSeed = Random.Range(0, MaxSeedValue);
        var topSeed = Random.Range(0, MaxSeedValue);
        var botSeed = Random.Range(0, MaxSeedValue);

        schemeIndex = (int)randScheme * SchemeOffset;

        var randHead = characterDesign.CharacterHeads[schemeIndex + headSeed];
        var randTop = characterDesign.CharacterHeads[schemeIndex + topSeed];
        var randBot = characterDesign.CharacterHeads[schemeIndex + botSeed];

        for (var i = 0; i < agentAi.meshList.Length; i++)
        {
            var index = i % 3;
            switch (index)
            {
                case 0:
                    headCount++;
                    if (headCount - 1 == headSeed)
                        SetMaterialAndActivate(agentAi.meshList[i], randHead.MaterialComponents);
                    break;
                case 1:
                    topCount++;
                    if (topCount - 1 == topSeed)
                        SetMaterialAndActivate(agentAi.meshList[i], randTop.MaterialComponents);
                    break;
                case 2:
                    botCount++;
                    if (botCount - 1 == botSeed)
                        SetMaterialAndActivate(agentAi.meshList[i], randBot.MaterialComponents);
                    break;
            }
        }
    }

    private void SetMaterialAndActivate(GameObject mesh, Material material)
    {
        foreach (var renderer in mesh.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = material;
        }
        mesh.SetActive(true);
    }
}