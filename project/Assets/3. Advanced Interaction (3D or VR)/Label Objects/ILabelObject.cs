using TMPro;
using UnityEngine;


namespace Sammoh.Three
{
// this is the base object for SnapPositionLabel and SnapRangeLabel
// this will be an abstract class instead because we want to be able to turn it into a prefab.
    public abstract class LabelObject : MonoBehaviour
    {

        [SerializeField] protected TMP_Text labelTextComponent;
        [SerializeField] protected float labelDistance;
        private string _labelText;

        public virtual void SetLabel(ISnapValue snapValue)
        {
            _labelText = snapValue.ID;
            labelTextComponent.text = _labelText;
        }

        public string GetLabel()
        {
            return _labelText;
        }

    }
}