using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Represents the world state including facts/blackboard and points of interest (POIs)
    /// </summary>
    public interface IWorldState
    {
        /// <summary>
        /// Get a world fact/property value
        /// </summary>
        bool GetFact(string factName);
        
        /// <summary>
        /// Set a world fact/property value
        /// </summary>
        void SetFact(string factName, bool value);
        
        /// <summary>
        /// Get an object reference from the blackboard
        /// </summary>
        T GetObject<T>(string key) where T : class;
        
        /// <summary>
        /// Set an object reference in the blackboard
        /// </summary>
        void SetObject<T>(string key, T value) where T : class;
        
        /// <summary>
        /// Find the nearest POI of a specific type to a given position
        /// </summary>
        GameObject FindNearestPOI(Vector3 position, string poiType);
        
        /// <summary>
        /// Find all POIs of a specific type
        /// </summary>
        List<GameObject> FindAllPOIs(string poiType);
        
        /// <summary>
        /// Register a new POI in the world
        /// </summary>
        void RegisterPOI(GameObject poi, NeedReductionGoalSO poiType);
        
        /// <summary>
        /// Unregister a POI from the world
        /// </summary>
        void UnregisterPOI(GameObject poi, string poiType);
        
        /// <summary>
        /// Check if a path exists between two positions (basic pathfinding check)
        /// </summary>
        bool IsPathClear(Vector3 from, Vector3 to);
        
        /// <summary>
        /// Get all facts as a dictionary for debugging/display
        /// </summary>
        Dictionary<string, bool> GetAllFacts();
    }
}