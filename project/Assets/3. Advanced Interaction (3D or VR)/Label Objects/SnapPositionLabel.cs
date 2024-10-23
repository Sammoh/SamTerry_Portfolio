using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.Three
{
    public class SnapPositionLabel : LabelObject
    {
        // this object should only use the parameters from the base class to build the label.

        [SerializeField] private float _labelWidth = 0.2f;

        // line renderer material.
        [SerializeField] private Material _lineRendererMaterial;

        public override void SetLabel(ISnapValue snapValue)
        {
            base.SetLabel(snapValue);

            if (snapValue is not SnapPosition snapPosition) return;

            var value = snapPosition.Value;
            // move the object to the correct position based on the heading.

            // get the heading from the snap angle based on the transform and the value.
            var heading = Quaternion.Euler(0, value, 0) * transform.forward;
            MoveToPosition(transform.position, heading);

            GenerateLineRenderer(heading);

            // move text to the correct position.
            labelTextComponent.transform.position = transform.position + heading * labelDistance;
            // turn to the side
            labelTextComponent.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        private void GenerateLineRenderer(Vector3 heading)
        {
            // create an object under this to add the line renderer to.
            GameObject lineRendererObject = new GameObject("LineRenderer");
            lineRendererObject.transform.parent = transform;

            LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + heading * labelDistance * 0.6f);
            lineRenderer.startWidth = _labelWidth;
            lineRenderer.alignment = LineAlignment.TransformZ;
            // flip to it's side.
            lineRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
            lineRenderer.material = _lineRendererMaterial;
        }

        // move the object to the correct position based on the heading.
        private void MoveToPosition(Vector3 position, Vector3 heading)
        {
            transform.position = position + heading * labelDistance;
        }

    }
}