using UnityEngine;

namespace Sammoh.Four
{
    public static class PlaneFactory
    {
        public static PlaneController CreatePlane(GameObject prefab, Transform spawn, Quaternion rotation,
            IControlStrategy controlStrategy, Vector3 offset = default)
        {
            var planeObject = Object.Instantiate(prefab, spawn.position + offset, rotation);
            var planeController = planeObject.GetComponent<PlaneController>();
            planeController.SetControlStrategy(controlStrategy);

            return planeController;
        }
    }
}