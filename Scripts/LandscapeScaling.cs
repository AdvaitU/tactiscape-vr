/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- LANDSCAPE SCALING SCRIPT ------------------------------------------

- SUMMARY: This script deals with scaling the landscape so that the user can see the changes they made on the model landscape at scale by traversing it using the VR setup.
- USED IN: (MeshInteractor script) Called in the main Update() loop when LeftPrimaryButton is pressed
- FOUND ON: 'Mesh Renderer' Game Object in Unity

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Experimental.GraphView.GraphView;

// ------------------------------------------------------------------------------------------------------------------------------------------------------------- START OF SCRIPT //






public class LandscapeScaling : MonoBehaviour
{
    // OBJECT REFERENCES ----------------------------------------------------------------------------------------

    public MeshGenerator _mesh;       // Reference provided in Unity Inspector to MeshGenerator.cs

    // PUBLIC MEMBERS -------------------------------------------------------------------------------------------

        /// None

    // PRIVATE MEMBERS ------------------------------------------------------------------------------------------
    private int scaleFactor = 200;

    /// Members that control scaling as a default Real World scale
    private bool isRealWorldScale = true;
    private const int realWorldScale = 200;

    /// Members for behaviour pattern i.e. Every toggle of the scale must be 2 seconds apart
    private bool isScaled = false;
    private bool isScalingAllowed = true;
    private float currTime;
    private const float timeBetweenScaling = 2.0f;


    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Start() : Sets the scale to realWorldScale if the associated boolean is enabled.
        - SetScaleRequested() : Multiplies the mesh's scale to scaleFactor
                              : (Overload) Sets it according to scaleFactor
        - SetScaleNormal() : Divides the mesh's scale by scaleFactor, effectively setting it to original (1, 1, 1)
        - *** ToggleScale() *** : Called at LeftPrimaryButton Press
                                : Toggles between scaling the mesh uniformly to (1) or (scaleFactor).
                                : Adjusts the visibility of the Selector spheres, movement of the character, clipping distance of the camera, and position of the character 
                                  accordingly

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // -----------------------
    public void Start()
    {
        if (isRealWorldScale) scaleFactor = realWorldScale;   // Set scale factor to realWorld if boolean is true
        _mesh = GetComponent<MeshGenerator>();                // Get the Mesh Generator Component
        currTime = Time.time;                                 // Start timer
    }

    // -----------------------
    public void SetScaleRequested()
    {
        _mesh.transform.localScale *= scaleFactor;
    }

    // -----------------------
    public void SetScaleRequested(int scaleFactor)
    {
        _mesh.transform.localScale *= scaleFactor;
    }

    // -----------------------
    public void SetScaleNormal()
    {
        _mesh.transform.localScale /= scaleFactor;
    }

    // *** ------------------------------ *** ------------------------------ *** ------- ToggleScale() ------- *** ------------------------------ *** -------------------------- //
    public void ToggleScale(MeshGenerator _mesh, int xSize, int zSize, ActionBasedContinuousMoveProvider _moveProvider, GameObject _player)
    {
    

        if (isScaled == false && isScalingAllowed)
        {
            SetScaleRequested();
            isScaled = true;
            isScalingAllowed = false;

            for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    _mesh.selectors[i].SetActive(false);
                }
            }
            _moveProvider.moveSpeed = 100.0f;
            _player.transform.position = new Vector3(0, -20, 0);
        }
        else if (isScaled && isScalingAllowed)
        {
            SetScaleNormal();
            isScaled = false;
            isScalingAllowed = false;

            for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    _mesh.selectors[i].SetActive(true);
                }
            }
            _moveProvider.moveSpeed = 1.0f;
            _player.transform.position = new Vector3(0, 0, 0);
        }
        else { /* Do Nothing */ }

        if (Time.time - currTime >= timeBetweenScaling)  // If 2 seconds have passed
        {
            isScalingAllowed = true;
            currTime = Time.time;
        }



    }

    // *** ------------------------------ *** ------------------------------ *** ------------------------------ *** ------------------------------ *** -------------------------- //

}









// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //

