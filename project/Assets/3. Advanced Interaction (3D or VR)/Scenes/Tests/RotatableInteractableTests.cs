using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Sammoh.Three;
using Assert = UnityEngine.Assertions.Assert;

public class RotatableKnobTests
{
    private GameObject knobObject;
    private RotatableKnob _interactable;
    
    [SetUp]
    public void Setup()
    {
        knobObject = new GameObject();
        _interactable = knobObject.AddComponent<RotatableKnob>();
        _interactable.SetSnappers(new List<ISnapValue>()
        {
            new SnapPosition("Off", 0f, 10f),
            new SnapPosition("Ignition", 15f, 10f),
            new SnapPosition("Test", 30f, 10f),
            new SnapRange("Range", 10f, 75f, 0.05f)
        });
    }
    
    [Test]
    public void GetValueID_AfterSettingToIgnition_ShouldReturnIgnition()
    {
        _interactable.SetValueID("Ignition");
        var valueID = _interactable.GetValueID();
        Assert.AreEqual("Ignition", valueID);
    }
    
    [Test]
    [TestCase(30f, ExpectedResult = (IEnumerator)null)]
    public IEnumerator SetValueID_AfterSettingToTest_ShouldReturnToOff_AfterTwoSeconds(float initialValue)
    {
        // get the "Test" snap value from the list.
        _interactable.SetValue(initialValue); // get the snap value from test.
        _interactable.OnEndInteraction();
        
        yield return new WaitForSeconds(2.5f);
        var valueID = _interactable.GetValueID();
        Assert.AreEqual("Off", valueID);
    }
    
    [Test]
    [TestCase(2, 15f)]
    public void SetValue_ToRange_ShouldSnapToRangeValue(int snapIndex, float expectedValue)
    {
        _interactable.SetValue(12.34f);
        Assert.AreEqual(expectedValue, _interactable.GetValue());
    }

    [Test] // This will make sure to account for the current list.
    [TestCase(0, 0)]
    [TestCase(1, 15f)]
    [TestCase(2, 30f)]
    public void SetValue_ShouldSnapToClosestValue(int snapPositionIndex, float expectedSnapValue)
    {
        var valueOffset = expectedSnapValue + 0.5f; // Slightly above the expected to test snapping
        _interactable.SetValue(valueOffset);
        Assert.AreEqual(expectedSnapValue, _interactable.GetValue());
    }

    [Test]
    [TestCase(9f, 0f)] // Test value just before the first snap position to ensure it snaps correctly
    public void SetValue_ShouldSnapToStartWhenValueIsJustBeforeFirstSnap(float inputValue, float expectedSnapValue)
    {
        _interactable.SetValue(inputValue);
        Assert.AreEqual(expectedSnapValue, _interactable.GetValue());
    }
}