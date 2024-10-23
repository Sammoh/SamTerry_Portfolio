3. Advanced Interaction (3D or VR)

Create the rotatable knob pictured with the following functionality.

• Knob values and positions:
    o Between 75 - 10 is free rotation, snap values to .05 increments
    o Off and Ignition are snapped positions
    o Test is a momentary, snapped position and changes to Off shortly after being set
• Create the following methods
    o SetValueID("Ignition”)
    o SetValue(22.1)
    o GetValueID()
    o GetValue()
• Events trigger when the values change while in use, also when it is done changing
• Create 2 more knobs with completely different values, different IDs at different positions
• Unit tests

this should have two modes. 
One that is going to be the vr implementation (faked or boilerplate)
The other mode, in editor, is for mouse logic. 

## New Stuff

mouse logic:
- click on the object.
- a slider is enabled.
- the slider should be labeled with the different modes.
- dragging the mouse left and right will interact with the object. 
- releasing the button will remove the interaction from the object. 

NOTES: Fix the snap distance so that it can break out of the snap range easier (see KnobRotation debug.)
