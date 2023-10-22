using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// ------------------------------------------------------------------------------- HAPTIC STRUCT ---------------------------------------------------------------------------------------------//
// Defined as class instead so that it may appear on Unity Inspector
[System.Serializable]
public class Haptics 
{
    [Range(0.0f, 1.0f)] [Tooltip("The intensity of the haptic feedback")]
    public float amplitude; // The intensity of the haptic feedback
    [Range(0.0f, 1.0f)] [Tooltip("The duration (in seconds) of the haptic feedback")]
    public float durationSeconds;  // The duration (in seconds) of the haptic feedback

    public bool isActive = false;   // If that particular haptic is active i.e. is being sent

    public Haptics(float amplitude, float durationSeconds)  // Constructor - Sets amplitude and durationSeconds as input it is initialised with
    {
        this.amplitude = amplitude;
        this.durationSeconds = durationSeconds;
    }
}
// ------------------------------------------------------------------------------- UPDATE() ---------------------------------------------------------------------------------------------//
public class HapticFeedback : MonoBehaviour
{
    // OBJECT REFERENCES ----------------------------------------------------------------
    public XRBaseController leftController, rightController;

    // PUBLIC MEMBERS -------------------------------------------------------------------
    [Header("Haptic Feedback Attributes")]

    public bool hapticFeedbackOn = true;          // Global setting for Haptic Feedback being on at all
    public Haptics touch = new(0.01f, 0.05f);     // Profile for if landscape is touched and not interacted with i.e. manipulated
    public Haptics interact = new(0.3f, 0.05f);   // Profile for if the landscape is interacted with

    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - SendHaptics(): Sends haptics according to the 'action' i.e. profile it is called with. Sends to Right controller by default
        - SendHaptics(): (Overload) Called by specifying the controller in addition to the profile
        - SendHaptics(): (Overload) Called with custom amplitude and durationSeconds values if the instantiated profiles don't fit the purpose

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // --------------------------------------------------------------------------
    public void SendHaptics(Haptics action)   // Defaults to rightController. Called as SendHaptics(touch)
    {
        rightController.SendHapticImpulse(action.amplitude, action.durationSeconds);
    }

    // --------------------------------------------------------------------------
    public void SendHaptics(XRBaseController controller, Haptics action)    // Additional option of controller. Called as SendHaptics(rightController, touch)
    {
        controller.SendHapticImpulse(action.amplitude, action.durationSeconds);
    }

    // --------------------------------------------------------------------------
    public void SendHaptics(XRBaseController controller, float amplitude, float durationSeconds)   // Additional options of custom amplitude and duration with defaults equal to interact Haptics object. Called as SendHaptics(rightController, 0.7f, 0.3f)
    {
        controller.SendHapticImpulse(amplitude, durationSeconds);
    }

}

// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //