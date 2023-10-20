using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;   // Namespace for the new Input System in Unity

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAction;   // Public variable so we can assign reference from Interaction Toolkit Input System through reference
    public InputActionProperty gripAction;

    public Animator handAnimator;    // Animator that's attached to hand by reference in Unity

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
