using UnityEngine;
/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                -------------------------------------- MESH INTERACTOR (MAIN SCRIPT) ------------------------------------------

- SUMMARY: This script allows for interaction with the mesh to deform it using multiple brush shapes, sizes, and impact types. It also instantiates and calls 
           all the other components of TactiScape in its Update Function
- USED IN: Is the one that uses.
- FOUND ON: 'Mesh Renderer' Game Object in Unity.

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

internal static class MeshInteractorHelpers
{

    private static bool AnyVelocityComponentHigherThan(Vector3 a, float margin)       // Checks if there is velocity in any direction
    {
        if (a.x > margin || a.y > margin || a.z > margin || a.x < -margin || a.y < -margin || a.z < -margin)   // If any of the 3 components are higher than the positive margin or lower than the negative margin i.e. movement above margin value in positive or negative direction
        {
            return true;   // Assume the hand has moved and return true to call haptics
        }
        else return false;  // Else assume the hand hasn't moved. margin is necessary as a limiter as the hand shakes a little anyway and we don't want the haptics to go off in that case
    }


    // ========================================================================================================================================================== END OF SECTION //

    // ------------------------------------------------------------------------------ CALCULATORS -------------------------------------------------------------------------------------------//
    private static float GetDistanceMagnitude(Vector3 a, Vector3 b)          // Get the Magnitue of distance between two vectors as a float
    {
        Vector3 vector = new(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }
}