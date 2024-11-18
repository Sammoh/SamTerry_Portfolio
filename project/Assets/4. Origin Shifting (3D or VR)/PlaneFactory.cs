using UnityEngine;

namespace Sammoh.Four
{
    public static class PlaneFactory
    {
        public static PlaneController CreatePlane(GameObject prefab, Vector3 position, Quaternion rotation,
            IControlStrategy controlStrategy)
        {
            var planeObject = Object.Instantiate(prefab, position, rotation);
            var planeController = planeObject.GetComponent<PlaneController>();
            planeController.SetControlStrategy(controlStrategy);

            return planeController;
        }
    }
}