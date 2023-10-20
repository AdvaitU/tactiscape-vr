using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// SUMMARY: This simple editor script regenerates plane when change is made. -------------------------------------------------

[CustomEditor(typeof(PlaneGenerator))]
public class PlaneGeneratorEditor : Editor
{
    /// <summary>
    /// Unity method called to draw inspector.
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Reference to the plane generator script.
        var generator = target as PlaneGenerator;
        // Regenerating plane only if we are in Play mode.
        if (Application.isPlaying)
        {
            generator.UpdatePlane();
        }
    }
}