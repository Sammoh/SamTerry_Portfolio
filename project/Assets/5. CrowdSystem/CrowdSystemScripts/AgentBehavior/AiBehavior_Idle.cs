using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBehavior_Idle : AIBehavior
{
    public float waitTime = 3f; // Editable in Inspector
    public GameObject ObjToDestroy;


    public override void Execute()
    {
        Debug.Log($"Executing Idle Behavior for {waitTime} seconds");
    }
}