/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- INPUT DATA SCRIPT ------------------------------------------

- SUMMARY: This script creates the Input Data objects necessary for capturing the actual data required to use the values in MeshInteractor.cs
- USED IN: (MeshInteractor script) Called in the main Update() loop when LeftSecondaryButton is pressed
- FOUND ON: 'Import/Export System' Game Object in Unity
- CREDIT: Thanks to 'Fist Full of Shrimp's Youtube tutorial and open source code. Availabe at: https://www.youtube.com/watch?v=Kh_94glqO-0

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputData : MonoBehaviour
{
    // OBJECT REFERENCES ----------------------------------------------------------------------------------------
    public InputDevice _rightController;   // Reference in Unity Inspector
    public InputDevice _leftController;    // Reference in Unity Inspector
    public InputDevice _HMD;               // Reference in Unity Inspector


    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Start() : Get the MeshGenerator script attached to the Mesh Renderer object in the scene.

        // PNG METHODS ---------------------
        - InitializeInputDevice() : Makes a list of devices based on available Input Devices supplied by OpenXR
        - InitializeInputDevices() : Calls the previous method on all the qavailable controllers i.e. Right, Left, and HMD
        - Update(): Unused in code. Checks for availibility of devices i.e. userPresence and reinitialises devices if connection has been lost in the middle

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // -----------------------------------------------------------------------------------
    private void InitializeInputDevice(InputDeviceCharacteristics inputCharacteristics, ref InputDevice inputDevice)
    {
        List<InputDevice> devices = new List<InputDevice>();
        //Call InputDevices to see if it can find any devices with the characteristics we're looking for
        InputDevices.GetDevicesWithCharacteristics(inputCharacteristics, devices);

        //Our hands might not be active and so they will not be generated from the search.
        //We check if any devices are found here to avoid errors.
        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
    }

    // -----------------------------------------------------------------------------------
    private void InitializeInputDevices()
    {

        if (!_rightController.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, ref _rightController);
        if (!_leftController.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, ref _leftController);
        if (!_HMD.isValid)
            InitializeInputDevice(InputDeviceCharacteristics.HeadMounted, ref _HMD);

    }

    

    // -----------------------------------------------------------------------------------
    void Update()
    {
        if (!_rightController.isValid || !_leftController.isValid || !_HMD.isValid)
            InitializeInputDevices();
    }

}
