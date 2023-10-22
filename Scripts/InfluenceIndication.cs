/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                -------------------------------------- INFLUENCE INDICATION SCRIPT ------------------------------------------

- SUMMARY: This script references 4 GameObjects in the scene (4 shapes of influence) and sets each active or inactive according to the brush being used
- USED IN: (MeshInteractor.cs) Called in Update() with SH_BrushShape category of functions.
- FOUND ON: 'InfluenceIndication' Game Object in Unity. GameObjects it references are attached to 'Right Hand' on 'VR Origin'

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// -------------------------------------------------------------------------------------------------------------------------------------------------------------START OF SCRIPT //
public class InfluenceIndication : MonoBehaviour
{
    // OBJECT REFERENCES ----------------------------------------------------------------------------------------
    [Header("Brush Shapes")]
    public GameObject _coneForward;               // Reference in Unity Inspector
    public GameObject _coneDown;                  // Reference in Unity Inspector
    public GameObject _sphere;                    // Reference in Unity Inspector
    public GameObject _hemisphereDown;            // Reference in Unity Inspector

    // PUBLIC MEMBERS -------------------------------------------------------------------------------------------
    public bool isVisible = true;      // True --> Influence Indication is visible in VR scene. False --> Invisible, but still instantiated. Use _influence.Destroy() if you want to permanently remove it from scene


    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Start() : Sets all the meshes as inactive at first frame
        - SetAllInactive() : Sets all the meshes as inactive
        - *** SetActiveBrush() *** : Used in Update() of MeshInteractor.cs within SH_BrushShape methods.
                                   : Sets all the meshes inactive first, then the right one active, and its local scale equal to brushRadius

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    private void Start()
    {
        SetAllInactive();
    }

    private void SetAllInactive()
    {
        _coneForward.SetActive(false);        // Set all as inactive
        _coneDown.SetActive(false);
        _sphere.SetActive(false);
        _hemisphereDown.SetActive(false);
    }

    public void SetActiveBrush(int i, float scale)
    {
        SetAllInactive();               // Set all as inactive
        if (i > 3) { return; }          // Quit if anything apart from 0, , 2, or 3 is passed as argument 1
        if (isVisible)                  // If global visibility is on
        {
            switch (i)
            {
                case 0:
                    _coneForward.SetActive(true);                                                // Set this brush shape active
                    _coneForward.transform.localScale = new Vector3(scale, scale, scale);        // Transform its scale according to brushRadius
                    break;
                case 1:
                    _coneDown.SetActive(true);
                    _coneDown.transform.localScale = new Vector3(scale, scale, scale);
                    break;
                case 2:
                    _sphere.SetActive(true);
                    _sphere.transform.localScale = new Vector3(scale, scale, scale);
                    break;
                case 3:
                    _hemisphereDown.SetActive(true);
                    _hemisphereDown.transform.localScale = new Vector3(scale, scale, scale);
                    break;
                default:
                    SetAllInactive();
                    break;
            }
        }
        else SetAllInactive();
        
        
    }

    // *** ------------------------------ *** ------------------------------ *** ------------------------------ *** ------------------------------ *** -------------------------- //

}

// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //
