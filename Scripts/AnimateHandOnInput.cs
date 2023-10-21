/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- HAND ANIMATION SCRIPT ------------------------------------------

- SUMMARY: This script animates the hands into a pinch and a fist according to trigger movements
- USED IN: (None) Directly on object
- FOUND ON: VR Origin Game Object in Unity

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   // Namespace for the new Input System in Unity


// ------------------------------------------------------------------------------------------------------------------------------------------------------------- START OF SCRIPT //
public class AnimateHandOnInput : MonoBehaviour
{

    // OBJECT REFERENCES ----------------------------------------------------------------------------------------
    public InputActionProperty pinchAction;   // Reference in Unity Inspector
    public InputActionProperty gripAction;    // Reference in Unity Inspector
    public Animator handAnimator;             // Reference in Unity Inspector

    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Update() : Updates the animator according to values of trigger and grip buttons, thus animating it

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    void Update()
    {
        // Pinch Animation with 'Trigger' button
        float triggerVal = pinchAction.action.ReadValue<float>(); // Uses action.ReadValue<type>() to get value from pinchAnim which is linked by reference to the Right Hand trigger
        handAnimator.SetFloat("Trigger", triggerVal);  // Use the animator to trigger the 'Trigger' animation relative to triggerVal
        //Debug.Log(triggerVal); // For debugging

        // Fist Animation with 'Grip' button
        float gripVal = gripAction.action.ReadValue<float>(); // Uses action.ReadValue<type>() to get value from pinchAnim which is linked by reference to the Right Hand trigger
        handAnimator.SetFloat("Grip", gripVal);


    }
}

// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //
