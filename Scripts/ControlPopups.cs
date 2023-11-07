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

    public void ShowAll()
    {
        show.SetActive(false);
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

    public void ShowSizeToggle(bool toggleOn)
    {
        increaseSize.SetActive(toggleOn);
        decreaseSize.SetActive(toggleOn);
        changeSize.SetActive(!toggleOn);
    }

}
