using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Helper to locate Points Of Interest via POIMarker using GOALS ONLY.
    /// </summary>
    public static class POIUtility
    {
        /// <summary>
        /// Find the nearest POI that supports the specified goal (no tag strings).
        /// </summary>
        public static bool TryGetNearestPOI(IGoal goal, Vector3 from, out Transform nearest)
        {
            nearest = null;
            float best = float.MaxValue;

            var markers = Object.FindObjectsOfType<POIMarker>();
            if (markers == null || markers.Length == 0 || goal == null) return false;

            for (int i = 0; i < markers.Length; i++)
            {
                var m = markers[i];
                if (m == null || !m.isActiveAndEnabled) continue;
                if (!m.SupportsGoal(goal)) continue;

                float d = (m.transform.position - from).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    nearest = m.transform;
                }
            }
            return nearest != null;
        }
    }
}