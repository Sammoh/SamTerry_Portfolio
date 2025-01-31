using UnityEngine;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    /// <summary>
    /// Design the agent's character by assigning random character designs.
    /// </summary>
    public class AgentFactory : MonoBehaviour
    {
        private Animator _animator;

        int schemeIndex = 0;
        private static readonly int MainTexture = Shader.PropertyToID("_Main_Texture");
        private static readonly int SkinTexture = Shader.PropertyToID("_Skin_Texture");

        private void SetMaterialAndActivate(MeshRenderer mesh, Texture2D material, Texture2D secondMaterial)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetTexture(MainTexture, material);
            props.SetTexture(SkinTexture, secondMaterial);
            
            // var rend = mesh.GetComponent<Renderer>();
            mesh.SetPropertyBlock(props);
            
            // todo should activate again.
            // mesh.SetActive(true);
        }

        public CrowdAgentAi CreateAgent(CharacterDesigns characterDesign, SpawnLocation spawnLocation, Transform spawnParent)
        {
            var agentObject = Instantiate(characterDesign.instancedCharacter, spawnLocation.transform.position, spawnLocation.transform.rotation, spawnParent);
            var spawnBehavior = spawnLocation.SpawnBehaviour ?? new AiBehavior_Idle(waitTime: 3);
            
            // add meshes to new agent
            var crowdAgent = agentObject.GetComponent<CrowdAgentAi>();
            crowdAgent.AddBehaviour(spawnBehavior);

            #region Design the agent's character by assigning random character designs.
            
            var characterMeshesRenderers = characterDesign.instancedCharacter.GetComponentsInChildren<MeshRenderer>();
            var selectedSkinColor = characterDesign.characterSkin[Random.Range(0, characterDesign.characterSkin.Length)];
            var maxSeedValue = characterDesign.characterColorVariants.Length;

            var headSeed = Random.Range(0, maxSeedValue);
            var topSeed = Random.Range(0, maxSeedValue);
            var botSeed = Random.Range(0, maxSeedValue);
            

            var randHead = characterDesign.characterColorVariants[headSeed];
            var randTop = characterDesign.characterColorVariants[topSeed];
            var randBot = characterDesign.characterColorVariants[botSeed];
            
            for (var i = 0; i < characterMeshesRenderers.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        SetMaterialAndActivate(characterMeshesRenderers[i], randHead, selectedSkinColor);
                        break;
                    case 1:
                        SetMaterialAndActivate(characterMeshesRenderers[i], randTop, selectedSkinColor);
                        break;
                    case 2:
                        SetMaterialAndActivate(characterMeshesRenderers[i], randBot, selectedSkinColor);
                        break;
                }
            }
            #endregion
            
            // create a new agent
            return crowdAgent;
        }
    }
}