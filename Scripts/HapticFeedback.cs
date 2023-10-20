using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// ------------------------------------------------------------------------------- HAPTIC STRUCT ---------------------------------------------------------------------------------------------//
// Defined as class instead so that it may appear on Unity Inspector
[System.Serializable]
public class Haptics 
{
    [Range(0.0f, 1.0f)]
    [Tooltip("The intensity of the haptic feedback")]
    public float amplitude; 
    [Range(0.0f, 1.0f)]
    [Tooltip("The duration (in seconds) of the haptic feedback")]
    public float durationSeconds;
    public bool isActive = false;   // If that particular haptic is active i.e. is being sent
    public Haptics(float amplitude, float durationSeconds)
    {
        this.amplitude = amplitude;
        this.durationSeconds = durationSeconds;
    }
}
// ------------------------------------------------------------------------------- UPDATE() ---------------------------------------------------------------------------------------------//
public class HapticFeedback : MonoBehaviour
{
    //private MeshGenerator _meshGenerator;
    public XRBaseController leftController, rightController;


    [Header("Haptic Feedback Attributes")]
    public bool hapticFeedbackOn = true;
    public Haptics touch = new(0.01f, 0.05f);
    public Haptics interact = new(0.3f, 0.05f);

    // ------------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------------------//
    /// <summary>
    /// Static method SendHaptics() + Its many overloads - Explained in each definition as comments
    /// </summary>

    public void SendHaptics(Haptics action)   // Defaults to rightController. Called as SendHaptics(touch)
    {
        rightController.SendHapticImpulse(action.amplitude, action.durationSeconds);
    }
    public void SendHaptics(XRBaseController controller, Haptics action)    // Additional option of controller. Called as SendHaptics(rightController, touch)
    {
        controller.SendHapticImpulse(action.amplitude, action.durationSeconds);
    }

    public void SendHaptics(XRBaseController controller, float amplitude, float durationSeconds)   // Additional options of custom amplitude and duration with defaults equal to interact Haptics object. Called as SendHaptics(rightController, 0.7f, 0.3f)
    {
        controller.SendHapticImpulse(amplitude, durationSeconds);
    }

    
}