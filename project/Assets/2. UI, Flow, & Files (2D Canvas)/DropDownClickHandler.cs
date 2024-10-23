using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles click events on dropdown elements.
/// </summary>

namespace Sammoh.Two
{
    public class DropDownClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public event Action Clicked;

        /// <summary>
        /// Called when the object is clicked.
        /// </summary>
        /// <param name="eventData">Information about the click event.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }
    }
}