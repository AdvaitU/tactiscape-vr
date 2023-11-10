/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                -------------------------------------- CONTROL POPUPS ------------------------------------------

- SUMMARY: This script has methods to show and hide 2D sprite popups based on user interactions
- USED IN: (MeshInteractor.cs) To set popups active and inactive based on user input
- FOUND ON: 'UI' Game Object in Unity.

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPopups : MonoBehaviour
{
    // Right Hand
    public GameObject trigger;
    public GameObject increaseSize;
    public GameObject changeSize;
    public GameObject changeImpact;
    public GameObject plRotation;
    

    // Left Hand
    public GameObject export;
    public GameObject decreaseSize;
    public GameObject scale;
    public GameObject changeShape;
    public GameObject plMovement;
    public GameObject show;
    public GameObject hide;

    // Show All - Sets All as active --------------------------------------------------------------------------------
    public void ShowAll()
    {
        show.SetActive(false);      // Sets the show sprite as inactive and replaces it with the hide sprite
        hide.SetActive(true);

        trigger.SetActive(true);
        changeSize.SetActive(true);
        changeImpact.SetActive(true);
        plRotation.SetActive(true);
        export.SetActive(true);
        scale.SetActive(true);
        changeShape.SetActive(true);
        plMovement.SetActive(true);


    }

    // Hides all the popups except 'Show Control' sprite
    public void HideAll()
    {
        show.SetActive(true);
        hide.SetActive(false);

        trigger.SetActive(false);
        //increaseSize.SetActive(false);
        changeSize.SetActive(false);
        changeImpact.SetActive(false);
        plRotation.SetActive(false);
        export.SetActive(false);
        //decreaseSize.SetActive(false);
        scale.SetActive(false);
        changeShape.SetActive(false);
        plMovement.SetActive(false);

    }

    // SHows size increase and decrease buttons when Size change button is pressed and held
    public void ShowSizeToggle(bool toggleOn)
    {
        increaseSize.SetActive(toggleOn);
        decreaseSize.SetActive(toggleOn);
        changeSize.SetActive(!toggleOn);
    }

}
