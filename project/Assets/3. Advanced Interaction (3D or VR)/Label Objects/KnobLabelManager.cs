using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.Three
{
	public class KnobLabelManager : MonoBehaviour
	{
		private RotatableKnob _rotatableKnob;

		// private readonly List<ISnapValue> _snapValues;
		private List<LabelObject> _labelObjects = new List<LabelObject>();

		[SerializeField] private Transform labelParent; // this is the parent object for the labels.

		[SerializeField]
		private float labelOffset; // this is the offset for the labels from the center towards the heading.

		[SerializeField]
		private SnapPositionLabel snapPositionPrefab; // this should be a label object in the form of a prefab.

		[SerializeField] private SnapRangeLabel snapRangePrefab;

		private List<ISnapValue> _snapValues = new();

		public void Initialize(RotatableKnob knob, List<ISnapValue> snapValues)
		{
			_rotatableKnob = knob;
			_snapValues = snapValues;


			// get the snaps. 
			// if the snap is a SnapPos, then get the position
			// if the snap is a range, then build out the range ui with a line renderer.
			//TODO ADD THIS BACK IN
			foreach (var snap in snapValues)
			{
				switch (snap)
				{
					case SnapPosition:
						// Spawn the snap position prefab.
						if (snapPositionPrefab != null)
						{
							SnapPositionLabel snapPositionLabel = Instantiate(snapPositionPrefab, labelParent);
							snapPositionLabel.SetLabel(snap);
							_labelObjects.Add(snapPositionLabel);
						}

						break;
					case SnapRange:
						if (snapRangePrefab != null)
						{
							var snapRangeLabel = Instantiate(snapRangePrefab, labelParent);
							snapRangeLabel.SetLabel(snap);
							_labelObjects.Add(snapRangeLabel);
						}

						break;
				}
			}
		}

		private void Update()
		{
			DrawDebugLinesForSnappedValues();
		}

		void DrawDebugLinesForSnappedValues()
		{
			float innerRadius = 0.5f; // Example inner radius
			float outerRadius = 1f; // Example outer radius
			Vector3 knobCenter = transform.position; // Assuming this script is attached to the knob
			float knobHeading = transform.rotation.eulerAngles.y;

			foreach (var snapPosition in _snapValues)
			{
				// Calculate direction based on the knob heading
				var headingOffset = knobHeading + snapPosition.Value;
				Vector3 direction = Quaternion.Euler(0, headingOffset, 0) * Vector3.forward;

				// Calculate start and end points
				Vector3 startPoint = knobCenter + direction * innerRadius;
				Vector3 endPoint = knobCenter + direction * outerRadius;

				// Draw the debug line
				Debug.DrawLine(startPoint, endPoint, Color.red,
					0.1f); // Last parameter is duration the line should be visible
			}
		}
	}
}