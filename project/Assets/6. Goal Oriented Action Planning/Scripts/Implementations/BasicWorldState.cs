using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Basic implementation of world state
    /// </summary>
    public class BasicWorldState : MonoBehaviour, IWorldState
    {
        [SerializeField] private bool debugMode = true;
        
        private Dictionary<string, bool> facts = new Dictionary<string, bool>();
        private Dictionary<string, object> blackboard = new Dictionary<string, object>();
        private Dictionary<string, List<GameObject>> pois = new Dictionary<string, List<GameObject>>();
        
        private void Awake()
        {
            // Initialize basic world facts
            facts["dayTime"] = true;
            facts["safe"] = true;
        }
        
        public bool GetFact(string factName)
        {
            return facts.TryGetValue(factName, out bool value) && value;
        }
        
        public void SetFact(string factName, bool value)
        {
            facts[factName] = value;
            if (debugMode)
                Debug.Log($"World fact '{factName}' set to {value}");
        }
        
        public T GetObject<T>(string key) where T : class
        {
            return blackboard.TryGetValue(key, out object value) ? value as T : null;
        }
        
        public void SetObject<T>(string key, T value) where T : class
        {
            blackboard[key] = value;
        }
        
        public GameObject FindNearestPOI(Vector3 position, string poiType)
        {
            if (!pois.ContainsKey(poiType))
                return null;
                
            GameObject nearest = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var poi in pois[poiType])
            {
                if (poi == null) continue;
                
                float distance = Vector3.Distance(position, poi.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = poi;
                }
            }
            
            return nearest;
        }
        
        public List<GameObject> FindAllPOIs(string poiType)
        {
            if (!pois.ContainsKey(poiType))
                return new List<GameObject>();
                
            // Remove null references
            pois[poiType].RemoveAll(poi => poi == null);
            return new List<GameObject>(pois[poiType]);
        }
        
        public void RegisterPOI(GameObject poi, string poiType)
        {
            if (poi == null) return;
            
            if (!pois.ContainsKey(poiType))
                pois[poiType] = new List<GameObject>();
                
            if (!pois[poiType].Contains(poi))
            {
                pois[poiType].Add(poi);
                if (debugMode)
                    Debug.Log($"Registered POI '{poi.name}' as type '{poiType}'");
            }
        }
        
        public void UnregisterPOI(GameObject poi, string poiType)
        {
            if (pois.ContainsKey(poiType))
            {
                pois[poiType].Remove(poi);
                if (debugMode)
                    Debug.Log($"Unregistered POI '{poi?.name}' from type '{poiType}'");
            }
        }
        
        public bool IsPathClear(Vector3 from, Vector3 to)
        {
            // Simple raycast check for basic obstacles
            return !Physics.Linecast(from, to, LayerMask.GetMask("Obstacle"));
        }
        
        public Dictionary<string, bool> GetAllFacts()
        {
            return new Dictionary<string, bool>(facts);
        }
        
        private void Start()
        {
            // Auto-register POIs in the scene
            RegisterExistingPOIs();
        }
        
        private void RegisterExistingPOIs()
        {
            // Find and register existing POIs in the scene
            var poiComponents = FindObjectsOfType<POIMarker>();
            foreach (var poi in poiComponents)
            {
                RegisterPOI(poi.gameObject, poi.POIType);
            }
        }
    }
    
    /// <summary>
    /// Component to mark GameObjects as Points of Interest
    /// </summary>
    public class POIMarker : MonoBehaviour
    {
        [SerializeField] public string POIType = "generic";
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(POIType))
                POIType = "generic";
        }
    }
}