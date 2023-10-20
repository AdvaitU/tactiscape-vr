using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceIndication : MonoBehaviour
{
    [Header("Influence Indicator Settings")]
    public Material brushInfluenceMaterial;       // Reference given in Unity Inspector - Transperant material around hand to show sphere of influence
    private GameObject influenceInstance;            // Object that shows sphere of influence
    public bool isVisible = true;

    public void CreateInfluenceInstance(Vector3 position, float radius)
    {
        // Setup sphere around right hand which shows the sphere of influence of the brush in use ------------------------------------------------
        influenceInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);           // Create sphere
        influenceInstance.GetComponent<Renderer>().material = brushInfluenceMaterial;   // Set material
        influenceInstance.GetComponent<Collider>().enabled = false;            // Disable collider or else character will keep colliding with it and keep moving backwards
        influenceInstance.transform.position = position;
        influenceInstance.transform.localScale = Vector3.one * radius;

    }

    // Constructor
    public void CreateInfluence(Vector3 position, float radius)
    {
        // Setup sphere around right hand which shows the sphere of influence of the brush in use ------------------------------------------------
        influenceInstance = GameObject.CreatePrimitive(PrimitiveType.Sphere);           // Create sphere
        influenceInstance.GetComponent<Renderer>().material = brushInfluenceMaterial;   // Set material
        influenceInstance.GetComponent<Collider>().enabled = false;            // Disable collider or else character will keep colliding with it and keep moving backwards
        influenceInstance.transform.position = position;
        influenceInstance.transform.localScale = Vector3.one * radius;

        influenceInstance.SetActive(isVisible);
    }

    // Update - Called on every frame
    // Transforming the brush influence according to position and rotation of hand, and size of brush
    public void UpdateInfluence(Vector3 position, Quaternion rotation, Vector3 hmdPosition, float radius)
    {
        influenceInstance.transform.SetPositionAndRotation(position, rotation);    // KEEP IN UPDATE() - To move sphere of influence being rendered in the scene
        if (influenceInstance.transform.localScale != Vector3.one * radius) influenceInstance.transform.localScale = Vector3.one * radius;  // Fix brushinfluence scale only if it doesn't match brush radius anymore
        if (isVisible) ToggleVisibility(hmdPosition, 0.1f);
    }

    private void ToggleVisibility(Vector3 hmdPosition, float minDistance)
    {
        if (Vector3.Distance(influenceInstance.transform.position, hmdPosition) < minDistance) influenceInstance.SetActive(false);
        else influenceInstance.SetActive(true);
    }

}
