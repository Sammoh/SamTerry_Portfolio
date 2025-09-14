using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Helper to locate Points Of Interest via POIMarker.
    /// Supports both legacy string tags and asset-driven goal matching.
    /// </summary>
    public static class POIUtility
    {
        // /// <summary>
        // /// Legacy string-tag lookup (e.g., "food", "water", "sleep", "toy").
        // /// </summary>
        // public static bool TryGetNearestPOI(string poiType, Vector3 from, out Transform nearest)
        // {
        //     nearest = null;
        //     float best = float.MaxValue;
        //
        //     var markers = Object.FindObjectsOfType<POIMarker>();
        //     if (markers == null || markers.Length == 0) return false;
        //
        //     for (int i = 0; i < markers.Length; i++)
        //     {
        //         var m = markers[i];
        //         if (m == null || !m.isActiveAndEnabled) continue;
        //
        //         if (!string.Equals(m.poiTag, poiType)) continue;
        //
        //         float d = (m.transform.position - from).sqrMagnitude;
        //         if (d < best)
        //         {
        //             best = d;
        //             nearest = m.transform;
        //         }
        //     }
        //
        //     return nearest != null;
        // }

        /// <summary>
        /// Asset-driven lookup: find nearest POI that supports the specified goal.
        /// </summary>
        public static bool TryGetNearestPOI(IGoal goal, Vector3 from, out Transform nearest)
        {
            nearest = null;
            float best = float.MaxValue;

            var markers = Object.FindObjectsOfType<POIMarker>();
            if (markers == null || markers.Length == 0) return false;

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
