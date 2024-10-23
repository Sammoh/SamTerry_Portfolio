using System;

namespace Sammoh.Three
{
    public interface IInteractable
    {
        void SetValueID(string id);
        void SetValue(float value);
        float GetValue();
        string GetValueID();
        float GetSnapValue(int index);
        event Action<float> OnValueChanged;
        event Action<float> OnValueChangeFinished;
        void OnStartInteraction();
        void OnEndInteraction();
    }
}