using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Simple script to create a basic test scene for GOAP demonstration
    /// This can be used to quickly setup the demo scene
    /// </summary>
    public class GOAPSceneSetup : MonoBehaviour
    {
        [Header("Scene Setup")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private Vector3 agentStartPosition = Vector3.zero;
        
        [Header("POI Positions")]
        [SerializeField] private Vector3 foodPosition = new Vector3(5, 0, 0);
        [SerializeField] private Vector3 waterPosition = new Vector3(-5, 0, 0);
        [SerializeField] private Vector3 toyPosition = new Vector3(0, 0, 5);
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupDemoScene();
            }
        }
        
        [ContextMenu("Setup Demo Scene")]
        public void SetupDemoScene()
        {
            Debug.Log("Setting up GOAP demo scene...");
            
            // Create world state
            CreateWorldState();
            
            // Create agent
            CreateAgent();
            
            // Create POIs
            CreatePOIs();
            
            Debug.Log("GOAP demo scene setup complete!");
        }
        
        private void CreateWorldState()
        {
            var existingWorldState = FindObjectOfType<BasicWorldState>();
            if (existingWorldState == null)
            {
                var worldStateGO = new GameObject("World State");
                worldStateGO.transform.position = Vector3.zero;
                worldStateGO.AddComponent<BasicWorldState>();
            }
        }
        
        private void CreateAgent()
        {
            var existingAgent = FindObjectOfType<GOAPAgent>();
            if (existingAgent == null)
            {
                // Create agent GameObject
                var agentGO = new GameObject("GOAP Agent");
                agentGO.transform.position = agentStartPosition;
                
                // Add visual representation (simple capsule)
                var meshRenderer = agentGO.AddComponent<MeshRenderer>();
                var meshFilter = agentGO.AddComponent<MeshFilter>();
                meshFilter.mesh = CreateCapsuleMesh();
                
                var material = new Material(Shader.Find("Standard"));
                material.color = Color.blue;
                meshRenderer.material = material;
                
                // Add collider for physics interactions
                agentGO.AddComponent<CapsuleCollider>();
                
                // Add the GOAP agent component
                agentGO.AddComponent<GOAPAgent>();
                
                Debug.Log("Created GOAP agent at " + agentStartPosition);
            }
        }
        
        private void CreatePOIs()
        {
            CreatePOI("Food Source", foodPosition, "food", Color.green);
            CreatePOI("Water Source", waterPosition, "water", Color.blue);
            CreatePOI("Toy", toyPosition, "toy", Color.yellow);
            CreatePOI("Bed", toyPosition, "bed", Color.grey);
        }
        
        private void CreatePOI(string name, Vector3 position, string poiType, Color color)
        {
            var poiGO = new GameObject(name);
            poiGO.transform.position = position;
            
            // Visual representation
            var meshRenderer = poiGO.AddComponent<MeshRenderer>();
            var meshFilter = poiGO.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateCubeMesh();
            
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetFloat("_Metallic", 0.2f);
            meshRenderer.material = material;
            
            // Add POI marker
            var poiMarker = poiGO.AddComponent<POIMarker>();
            // new NeedReductionGoalSO
            var goalAsset = Resources.Load<NeedReductionGoalSO>($"Goals/{poiType.ToLower()}_Goal");
            if (goalAsset == null)
                poiMarker.AddSupportedGoal(goalAsset);
            
            // Add collider
            poiGO.AddComponent<BoxCollider>();
            
            Debug.Log($"Created POI '{name}' of type '{poiType}' at {position}");
        }
        
        private Mesh CreateCapsuleMesh()
        {
            // For simplicity, create a basic mesh
            // In a real implementation, you'd use Unity's built-in meshes or assets
            var mesh = new Mesh();
            
            // Simple quad for demonstration
            Vector3[] vertices = {
                new Vector3(-0.5f, -1f, -0.5f),
                new Vector3(0.5f, -1f, -0.5f),
                new Vector3(0.5f, 1f, -0.5f),
                new Vector3(-0.5f, 1f, -0.5f),
                new Vector3(-0.5f, -1f, 0.5f),
                new Vector3(0.5f, -1f, 0.5f),
                new Vector3(0.5f, 1f, 0.5f),
                new Vector3(-0.5f, 1f, 0.5f)
            };
            
            int[] triangles = {
                0, 2, 1, 0, 3, 2, // Front
                1, 6, 5, 1, 2, 6, // Right
                5, 7, 4, 5, 6, 7, // Back
                4, 3, 0, 4, 7, 3, // Left
                3, 6, 2, 3, 7, 6, // Top
                0, 5, 4, 0, 1, 5  // Bottom
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
            return mesh;
        }
        
        private Mesh CreateCubeMesh()
        {
            var mesh = new Mesh();
            
            Vector3[] vertices = {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            };
            
            int[] triangles = {
                0, 2, 1, 0, 3, 2, // Front
                1, 6, 5, 1, 2, 6, // Right
                5, 7, 4, 5, 6, 7, // Back
                4, 3, 0, 4, 7, 3, // Left
                3, 6, 2, 3, 7, 6, // Top
                0, 5, 4, 0, 1, 5  // Bottom
            };
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
            return mesh;
        }
    }
}