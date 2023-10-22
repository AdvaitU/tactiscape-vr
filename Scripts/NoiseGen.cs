/*------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- NOISE GENERATOR SCRIPT ------------------------------------------

- SUMMARY: Generates noise according to Perlin Noise 
- USED IN: (MeshInteractor.cs) Called in the main Update() loop when LeftSecondaryButton is pressed
- FOUND ON: 'MeshRenderer' Game Object in Unity
- CREDIT: Brackeys' YouTube tutorial and code. Found at: https://www.youtube.com/watch?v=bG0uEXV6aHQ

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseLayer : MonoBehaviour
{
    [SerializeField] private float noisePower = 1; // Multiplier for noise. 
    [SerializeField] private Vector2 noiseOffset;  // Noise offset.
    [SerializeField] private float noiseScale = 1; // Noise scale.


    // METHOD (SINGULAR) ---------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Evaluate value for X and Y coords. Please excuse the typo in the script name now, it's too late to change it. </summary>
    /// <returns> Returns float value from noise.</returns>
    /// <param name="x"> The x coordinate (0.0-1.0).</param>
    /// <param name="y"> The y coordinate (0.0-1.0).</param>
    public float Elevate(float x, float y)
    {
        // Adding elevation from perlin noise.
        float noiseXCoord = noiseOffset.x + x * noiseScale;        // Add noise at set scale to x and y input parameters along with noise offset from x and y.
        float noiseYCoord = noiseOffset.y + y * noiseScale;
        return (Mathf.PerlinNoise(noiseXCoord, noiseYCoord) - 0.5f) * noisePower;      // Return Perlin noise value for newly calculated point multiplied by amplitude.
    }
}

// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //
