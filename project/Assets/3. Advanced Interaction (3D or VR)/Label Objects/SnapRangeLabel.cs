using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.Three
{
    public class SnapRangeLabel : LabelObject
    {
        // this object should only use the parameters from the base class to build the label.
        // this object would have an arc renderer to show the range.
        // this object would have a text component to show the range for the start and end. 
        // this object will use a line renderer to show the range.
        // this object will use an image as a pipe to show the distance from the center of the knob.

        // [SerializeField] private float rangeHeight;
        // [SerializeField] private float rangeDepth;
        [SerializeField] private float rangeRadius;

        // [SerializeField] private float rangeAngle;
        [SerializeField] private float rangeResolution;

        // [SerializeField] private float rangeColor;
        [SerializeField] private Material rangeMaterial;

        [SerializeField] private float _labelWidth = 0.2f;

        // line renderer material.
        [SerializeField] private Material _lineRendererMaterial;

        [SerializeField] private float _lineRendererScale = 1;
        [SerializeField] private int _lineRendererResolution = 10;

        private float _rangeStart;
        private float _rangeEnd;

        public override void SetLabel(ISnapValue snapValue)
        {
            base.SetLabel(snapValue);

            if (snapValue is not SnapRange snapRange) return;

            _rangeStart = snapRange.MinValue;
            _rangeEnd = snapRange.MaxValue;

            // move the object to the correct position based on the heading.

            // get the heading from the snap angle based on the transform and the value.
            var minHeading = Quaternion.Euler(0, _rangeStart, 0) * transform.forward;
            var maxHeading = Quaternion.Euler(0, _rangeEnd, 0) * transform.forward;

            // set the range for the line renderer from the min and max values.
            SetRange();

            // generate line renderers for the min and max headings.
            var minLine = GenerateLineRenderer(minHeading);
            var maxLine = GenerateLineRenderer(maxHeading);

            // move the line to the correct position.
            minLine.transform.position = transform.position;
            maxLine.transform.position = transform.position;

            // move the min line towards the min heading.
            minLine.transform.position = transform.position + minHeading * labelDistance;
            minLine.transform.position = transform.position + maxHeading * labelDistance;
        }

        private LineRenderer GenerateLineRenderer(Vector3 heading)
        {

            // create an object under this to add the line renderer to.
            GameObject lineRendererObject = new GameObject("LineRenderer");
            lineRendererObject.transform.parent = transform;
            lineRendererObject.transform.position = transform.position;

            LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position + heading * labelDistance);
            lineRenderer.SetPosition(1, transform.position + heading * labelDistance * 1.6f);
            lineRenderer.startWidth = _labelWidth;
            lineRenderer.alignment = LineAlignment.TransformZ;
            // flip to it's side.
            lineRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
            lineRenderer.material = _lineRendererMaterial;
            return lineRenderer;
        }

        // set the range for the line renderer from the min and max values.
        private void SetRange()
        {
            // create an object under this to add the line renderer to.
            GameObject lineRendererObject = new GameObject("LineRenderer");
            LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();

            var heading = Quaternion.Euler(0, (_rangeStart + _rangeEnd) / 2, 0) * transform.forward;

            lineRendererObject.transform.parent = transform;
            // move towards the eading.
            lineRendererObject.transform.position = transform.position + heading * labelDistance;


            // add a gradient material to the line renderer.
            lineRenderer.material = rangeMaterial;

            lineRenderer.alignment = LineAlignment.TransformZ;

            // set the range for the line renderer from the min and max values.
            lineRenderer.startWidth = _lineRendererScale;
            lineRenderer.endWidth = _lineRendererScale * 0.1f;

            lineRenderer.positionCount = (int)rangeResolution;

            Debug.Log($"Range start: {_rangeStart}, Range end: {_rangeEnd}");

            // set the range for the line renderer from the min and max values.
            for (int i = 0; i < rangeResolution; i++)
            {
                var angleOffset = 10f;
                var angle = Mathf.Lerp(_rangeStart, _rangeEnd, i / rangeResolution);
                var position = transform.position + Quaternion.Euler(0, angle + angleOffset, 0) * transform.forward *
                    rangeRadius * 0.75f;
                lineRenderer.SetPosition(i, position);
            }

            lineRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);

        }
    }
}