using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Noise layer.
/// </summary>
[System.Serializable]
public class NoiseLayer : MonoBehaviour
{
    // Multiplier for noise.
    [SerializeField]
    private float noisePower = 1;
    // Noise offset.
    [SerializeField]
    private Vector2 noiseOffset;
    // Noise scale.
    [SerializeField]
    private float noiseScale = 1;
    /// <summary>
    /// Evalate value for X and Y coords.
    /// </summary>
    /// <returns>Returns value from noise.</returns>
    /// <param name="x">The x coordinate (0.0-1.0).</param>
    /// <param name="y">The y coordinate (0.0-1.0).</param>
    public float Elevate(float x, float y)
    {
        // Adding elevation from perlin noise.
        float noiseXCoord = noiseOffset.x + x * noiseScale;
        float noiseYCoord = noiseOffset.y + y * noiseScale;
        return (Mathf.PerlinNoise(noiseXCoord, noiseYCoord) - 0.5f) * noisePower;
    }
}
