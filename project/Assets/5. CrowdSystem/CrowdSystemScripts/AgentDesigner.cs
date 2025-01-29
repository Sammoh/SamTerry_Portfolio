using System;
using System.Collections.Generic;
using Sammoh.CrowdSystem;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sammoh.CrowdSystem
{
    /// <summary>
    /// Design the agent's character by assigning random character designs.
    /// </summary>
    public class AgentDesigner : MonoBehaviour
    {
        private Animator _animator;

        int schemeIndex = 0;
        private static readonly int MainTexture = Shader.PropertyToID("_Main_Texture");
        private static readonly int SkinTexture = Shader.PropertyToID("_Skin_Texture");

        public void AssignRandomCharacter(CrowdAgentAi agentAi, CharacterDesigns characterDesign)
        {
            // #region MyRegion
            //
            // var selectedSkinColor = characterDesign.characterSkin[Random.Range(0, characterDesign.characterSkin.Length)];
            // var MaxSeedValue = characterDesign.characterColorVariants.Length;
            // // var SchemeOffset = 3; // this offset is used to group types together, eg. light, dark, etc.
            //
            // var randScheme = (ColorScheme)Random.Range(0, MaxSeedValue);
            // var headSeed = Random.Range(0, MaxSeedValue);
            // var topSeed = Random.Range(0, MaxSeedValue);
            // var botSeed = Random.Range(0, MaxSeedValue);
            //
            //
            // var randHead = characterDesign.characterColorVariants[headSeed];
            // var randTop = characterDesign.characterColorVariants[topSeed];
            // var randBot = characterDesign.characterColorVariants[botSeed];
            //
            // for (var i = 0; i < agentAi.MeshList.Length; i++)
            // {
            //     switch (i)
            //     {
            //         case 0:
            //             SetMaterialAndActivate(agentAi.MeshList[i], randHead, selectedSkinColor);
            //             break;
            //         case 1:
            //             SetMaterialAndActivate(agentAi.MeshList[i], randTop, selectedSkinColor);
            //             break;
            //         case 2:
            //             SetMaterialAndActivate(agentAi.MeshList[i], randBot, selectedSkinColor);
            //             break;
            //     }
            // }
            //
            // #endregion
        }

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

        public CrowdAgentAi CreateAgent<T>(CharacterDesigns characterDesign, T behaviourType) where T : AIBehavior
        {
            // creating a new agent from a GO
            var characterMeshesRenderers = characterDesign.instancedCharacter.GetComponentsInChildren<MeshRenderer>();

            // add meshes to new agent
            
            
            var newCrowdAgent = new GameObject("CrowdAgent").AddComponent<CrowdAgentAi>();
            
            // TODO instantiate the agent's mesh from prefab?

            #region MyRegion

            var selectedSkinColor = characterDesign.characterSkin[Random.Range(0, characterDesign.characterSkin.Length)];
            var MaxSeedValue = characterDesign.characterColorVariants.Length;
            // var SchemeOffset = 3; // this offset is used to group types together, eg. light, dark, etc.

            var randScheme = (ColorScheme)Random.Range(0, MaxSeedValue);
            var headSeed = Random.Range(0, MaxSeedValue);
            var topSeed = Random.Range(0, MaxSeedValue);
            var botSeed = Random.Range(0, MaxSeedValue);
            

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
            return newCrowdAgent;
        }
    }
}