using UnityEngine;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    /// <summary>
    /// Design the agent's character by assigning random character designs.
    /// </summary>
    public class AgentFactory : MonoBehaviour
    {
        int schemeIndex = 0;
        private static readonly int MainTexture = Shader.PropertyToID("_Main_Texture");
        private static readonly int SkinTexture = Shader.PropertyToID("_Skin_Texture");
        
        private void SetMaterialAndActivate(Renderer mesh, Texture2D material, Texture2D skinMaterial)
        {
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetTexture(MainTexture, material);
            props.SetTexture(SkinTexture, skinMaterial);
            
            mesh.SetPropertyBlock(props);
        }

        public CrowdAgentAi CreateAgent(CharacterDesigns characterDesign, SpawnLocation spawnLocation, Transform spawnParent, int index)
        {
            // this is the base object that will be used...
            var agentObject = Instantiate(characterDesign.instancedCharacter, spawnLocation.transform.position, spawnLocation.transform.rotation, spawnParent);
            var randomValue = Random.Range(0, agentObject.AgentParts.CharacterTorsos.Length);
            var randomHead = agentObject.AgentParts.CharacterHeads[randomValue];
            var randomTorso = agentObject.AgentParts.CharacterTorsos[randomValue];
            var randomLegs = agentObject.AgentParts.CharacterLegs[randomValue];
            var selectedSkinColor = characterDesign.characterSkin[Random.Range(0, characterDesign.characterSkin.Length)];

            var hair = agentObject.AgentParts.AddHair(randomValue);

            // turn off all other gameobjects except the selected ones.
            foreach (var head in agentObject.AgentParts.CharacterHeads)
            {
                head.SetActive(head == randomHead);

                if (head != randomHead) continue;
                
                var headRenderer = head.GetComponent<Renderer>();
                var selectedHeadColor = characterDesign.characterColorVariants[Random.Range(0, characterDesign.characterColorVariants.Length)];
                SetMaterialAndActivate(headRenderer, selectedHeadColor, selectedSkinColor);
            }
            foreach (var torso in agentObject.AgentParts.CharacterTorsos)
            {
                torso.SetActive(torso == randomTorso);

                if (torso != randomTorso) continue;
                
                var torsoRenderer = torso.GetComponent<Renderer>();
                var selectedTorsoColor = characterDesign.characterColorVariants[Random.Range(0, characterDesign.characterColorVariants.Length)];
                SetMaterialAndActivate(torsoRenderer, selectedTorsoColor, selectedSkinColor);
            }
            foreach (var legs in agentObject.AgentParts.CharacterLegs)
            {
                legs.SetActive(legs == randomLegs);

                if (legs != randomLegs) continue;
                
                var legsRenderer = legs.GetComponent<Renderer>();
                var selectedLegsColor = characterDesign.characterColorVariants[Random.Range(0, characterDesign.characterColorVariants.Length)];
                SetMaterialAndActivate(legsRenderer, selectedLegsColor, selectedSkinColor);
            }
            // activate hair
            
            
            var spawnBehavior = spawnLocation.SpawnBehaviour ?? new AiBehavior_Idle(waitTime: 3);
            
            // add meshes to new agent
            var crowdAgent = agentObject.GetComponent<CrowdAgentAi>();
            crowdAgent.AddBehaviour(spawnBehavior);
            
            // create a new agent
            return crowdAgent;
        }
    }
}