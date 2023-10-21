using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(InputData))]
public class MeshInteractor : MonoBehaviour
{
    // ------------------------------------------------------------------------------- VARIABLES ---------------------------------------------------------------------------------------------//
    private InputData _inputData;                      // Reference in Start() - Accessing inputData script which initialises and names HMD, right and left controllers
    public GameObject _player;                         // Reference given in Unity Inspector  - Reference to VR origin (Player)
    private CharacterController _characterController;  // Reference in Start() - To access the position of the character ion the world space i.e. wrt vertices
    private MeshGenerator _meshGenerator;              // Reference in Start() - Mesh Generation script which updates and stores info on vertices, triangles, uvs, and normals
    private HapticFeedback _hapticFeedback;            // Reference in Start() - HapticFeedback script that contains methods to send Haptic Feedback to right or left controllers
    private ActionBasedContinuousMoveProvider _moveProvider;   // Reference in Start()
    public Transform _rightForward;

    [Header("Brush Settings")]
    [Range(0.0f, 3.0f)]
    public float brushRadius = 0.3f;              // Brush's Radius - For spherical basic brushy
    [Range(0.0f, 5.0f)]
    public float brushStrength = 1.0f;            // Magnitude with which vertices are pushed
    public bool strengthProportionalToRadius = true;

    public GameObject _influenceIndication;         // Reference to Import_Export Game Object set in Unity Inspector
    private InfluenceIndication _influence;



    public GameObject _import_export;         // Reference to Import_Export Game Object set in Unity Inspector
    private Import_Export _exporter;          // Instance of Import_Export Script

    private float currTime;
    private bool sendHapticFeedbackInteract = false;   // Starts as false. If true, Haptic Feedback is sent to the user
    private bool sendHapticFeedbackTouch = false;   // Starts as false. If true, Haptic Feedback is sent to the user

    public LandscapeScaling _landscapeScaling;



    // -------------------------------------------------------------------------------- START() ---------------------------------------------------------------------------------------------//

    private void Start()
    {
        _meshGenerator = GetComponent<MeshGenerator>();    // Get the MeshGenerator script attached to this object
        _inputData = GetComponent<InputData>();            // Get the InputData script attached to this object
        _characterController = _player.GetComponent<CharacterController>();   // Get the CharacterController script attached to the VR Origin object in the scene
        _exporter = _import_export.GetComponent<Import_Export>();             // Get the Import_Export script from the Import_Export object
        _hapticFeedback = GetComponent<HapticFeedback>();                     // Get Haptic Feedback script
        _influence = _influenceIndication.GetComponent<InfluenceIndication>(); // Get Influence Indication Script
        _landscapeScaling = GetComponent<LandscapeScaling>();
        _moveProvider = _player.GetComponent<ActionBasedContinuousMoveProvider>();



        _influence.CreateInfluence(GetCharPos(), brushRadius);
        currTime = Time.time;

        if (strengthProportionalToRadius) brushStrength = brushRadius;
    }

    // ------------------------------------------------------------------------------- UPDATE() ---------------------------------------------------------------------------------------------//
    void Update()
    {

        // Debug Testers - Log if Right Hand controller buttons are pressed
        //if (GetRightGrip()) Debug.Log("Gripped");
        //if (GetRightTrigger()) Debug.Log("Triggered");
        //Debug.Log(GetRightVel());

        // Creating local variables as copies of corresponding variables/arrays from Mesh Generator class
        Vector3[] newVertices = _meshGenerator.vertices;
        int[] newTriangles = _meshGenerator.triangles;
        UnityEngine.Color[] newColours = _meshGenerator.colours;
        int xSize = _meshGenerator.xSize;
        int zSize = _meshGenerator.zSize;
        float minHeight = _meshGenerator.minHeight;
        float maxHeight = _meshGenerator.maxHeight;

        // EXPORTER - Check if button is pressed every frame and call the Exporter to export at set settings if it is.
        if (GetLeftSecondaryButton()) _exporter.ExportScene(_meshGenerator);
        if (GetLeftPrimaryButton()) _landscapeScaling.ToggleScale(_meshGenerator, xSize, zSize, _moveProvider, _player);

        // Haptic Feedback
        sendHapticFeedbackInteract = false; /* AND */ sendHapticFeedbackTouch = false;   // Set as false at the beginning of every frame

        // ------------------------------------------------------------------------ AVERAGE BRUSHES ----------------------------------------------------------------------------- //


        List<Vector3> relevantVertices = new();
        Vector3 aggregateVertices = Vector3.zero;




        for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                if (SH_FrontOfCone(ref newVertices, ref i))  // If a vertices is within brushRadius distance from hand
                {

                    relevantVertices.Add(newVertices[i]);

                    if (GetRightTrigger())   // If right trigger is pressed
                    {
                        foreach (Vector3 v in relevantVertices)
                        {
                            aggregateVertices += v;
                        }
                        aggregateVertices /= relevantVertices.Count;
                        Vector3 directionToAverage = (GetRightPos() - aggregateVertices).normalized;
                        newVertices[i] += brushStrength * Time.deltaTime * -directionToAverage * GetRightVelMagnitude();
                        _meshGenerator.selectors[i].transform.position = newVertices[i];  // Move the selector spheres to the location of the new vertices
                    }

                    else if (GetRightGrip())    // If right grip is pressed
                    {
                        //B_PullUp(ref newVertices, ref i);
                    }
                }

                if (newVertices[i].y > maxHeight) maxHeight = newVertices[i].y;      // Update minHeight and maxHeight according to y generated using Perlin Noise
                if (newVertices[i].y < minHeight) minHeight = newVertices[i].y;      // At the end, we should have a variable measure of the max height and the min height in the generation.
            }
        }

        relevantVertices.Clear();




        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- //




        // --------------------------------------------------------------------- NON AVERAGE BRUSHES ------------------------------------------------------------------------- //


        /*for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                if (SH_UnderCone(ref newVertices, ref i))  // If a vertices is within brushRadius distance from hand
                {

                    if (GetRightTrigger())   // If right trigger is pressed
                    {
                        B_PushDown(ref newVertices, ref i);
                    }

                    else if (GetRightGrip())    // If right grip is pressed
                    {
                        B_PullUp(ref newVertices, ref i);
                    }
                }

                if (newVertices[i].y > maxHeight) maxHeight = newVertices[i].y;      // Update minHeight and maxHeight according to y generated using Perlin Noise
                if (newVertices[i].y < minHeight) minHeight = newVertices[i].y;      // At the end, we should have a variable measure of the max height and the min height in the generation.
            }
        }*/


        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- //


        // Haptic Feedback check
        if (!_hapticFeedback.hapticFeedbackOn) { /* Do nothing */ }
        else if (sendHapticFeedbackInteract) _hapticFeedback.SendHaptics(_hapticFeedback.interact);  // Interact has priority, send if interact is true
        else if (sendHapticFeedbackTouch) _hapticFeedback.SendHaptics(_hapticFeedback.touch);   // Else check touch and send touch
        else { /* Do nothing */ }

        //Debug.Log(GetRightForwardVector());

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.Abs(1 - Mathf.InverseLerp(minHeight, maxHeight, newVertices[i].y));   // Lerp between maximum and minimum height and give value between 0 and 1
                newColours[i] = _meshGenerator.gradient.Evaluate(height);
                i++;
            }
        }


        // Copying newVertices, newTriangles, and newColours back into MeshGenerator script arrays
        _meshGenerator.vertices = newVertices;
        _meshGenerator.triangles = newTriangles;
        _meshGenerator.colours = newColours;

        _influence.UpdateInfluence(GetRightPos(), GetCharRot() * GetRightRot(), GetCharPos(), brushRadius);

        if (strengthProportionalToRadius) if (brushStrength != brushRadius) brushStrength = brushRadius;


    }

    // ------------------------------------------------------------------------------- FUNCTIONS ---------------------------------------------------------------------------------------------//

    // GETTERS AND CHECKERS
    /*
     * All of these act as Getters or Checkers for various properties of the hand wrt to the world it exists in. For eg: The GetRightPos() method takes into account the distance of the 
     * Right Hand Controller from the HMD which is directly linked to the player's position in the world and adds the two Vector3s to give the position of the hand in the world.
     * 
     */

    // Position, Rotation, Velocity, etc. ------------------------------------------------------------------------
    // Character ----------------------------------------------------
    Vector3 GetCharPos()
    {
        return _characterController.transform.position;
    }

    Quaternion GetCharRot()
    {
        return _characterController.transform.rotation;
    }

    // Right Hand ----------------------------------------------------
    Vector3 GetRightPos()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos)) return GetCharPos() + rightPos;
        else return new Vector3(0, 0, 0);
    }

    Quaternion GetRightRot()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRot)) return rightRot;
        else return new Quaternion(1, 1, 1, 1);
    }

    Vector3 GetRightVel()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVel)) return rightVel;
        else return new Vector3(0, 0, 0);
    }

    float GetRightVelMagnitude()
    {
        Vector3 v = GetRightVel();
        return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
    }

    Vector3 GetRightForwardVector()
    {
        return _rightForward.transform.forward;
    }

    // Left Hand ----------------------------------------------------
    Vector3 GetLeftPos()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos)) return GetCharPos() + leftPos;
        else return new Vector3(0, 0, 0);
    }

    Quaternion GetLeftRot()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRot)) return leftRot;
        else return new Quaternion(1, 1, 1, 1);
    }

    Vector3 GetLeftVel()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVel)) return leftVel;
        else return new Vector3(0, 0, 0);
    }

    // Button Presses ---------------------------------------------------------------------------------------------------
    // Right Hand ----------------------------------------------------
    bool GetRightGrip()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.grip, out float gripVal))
        {
            if (gripVal > 0.6f) return true;
            else return false;
        }
        else return false;
    }

    // Left Hand ----------------------------------------------------
    bool GetRightTrigger()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out float trigVal))
        {
            if (trigVal > 0.6f) return true;
            else return false;
        }
        else return false;
    }

    bool GetLeftSecondaryButton()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isSecondary) && isSecondary) return true;
        else return false;
    }

    bool GetLeftPrimaryButton()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPrimary) && isPrimary) return true;
        else return false;
    }


    // ------------------------------------------------------------------------------ CALCULATORS -------------------------------------------------------------------------------------------//
    public static float GetDistanceMagnitude(Vector3 a, Vector3 b)
    {
        Vector3 vector = new(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    public static bool AnyVelocityComponentHigherThan(Vector3 a, float margin)
    {
        if (a.x > margin || a.y > margin || a.z > margin || a.x < -margin || a.y < -margin || a.z < -margin)   // If any of the 3 components are higher than the positive margin or lower than the negative margin i.e. movement above margin value in positive or negative direction
        {
            return true;   // Assume the hand has moved and return true to call haptics
        }
        else return false;  // Else assume the hand hasn't moved. margin is necessary as a limiter as the hand shakes a little anyway and we don't want the haptics to go off in that case
    }


    // ------------------------------------------------------------------------------- BRUSH SHAPES ---------------------------------------------------------------------------------------------//

    private bool SH_Sphere(ref Vector3[] vertices, ref int i)
    {

        if (Vector3.Distance(vertices[i], GetRightPos()) <= brushRadius) return true;
        else return false;
    }
    private bool SH_HemisphereUp(ref Vector3[] vertices, ref int i)
    {
        if (Vector3.Distance(vertices[i], GetRightPos()) <= brushRadius && (GetRightPos().y <= vertices[i].y)) return true;
        else return false;
    }

    private bool SH_HemisphereDown(ref Vector3[] vertices, ref int i)
    {
        if (Vector3.Distance(vertices[i], GetRightPos()) <= brushRadius && (GetRightPos().y >= vertices[i].y))
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(GetRightVel(), 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }
    }

    private bool SH_FrontOfCone(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = GetRightForwardVector();
        Vector3 directiontoVertice = (GetRightPos() - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);
        

        if (Vector3.Distance(vertices[i], GetRightPos()) <= brushRadius && (dotProductOfVectors > 0.5))
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(GetRightVel(), 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            Debug.Log(dotProductOfVectors);
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }

    private bool SH_UnderCone(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = GetRightForwardVector();
        Vector3 directiontoVertice = (GetRightPos() - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);


        if (Vector3.Distance(vertices[i], GetRightPos()) <= brushRadius && (dotProductOfVectors < 0.3 || dotProductOfVectors > -0.3))
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(GetRightVel(), 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            Debug.Log(dotProductOfVectors);
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }



    // ------------------------------------------------------------------------------- BRUSHES ---------------------------------------------------------------------------------------------//


    /// <summary>
    /// Radial Push and Pull Brushes
    /// </summary>
    private void B_PushRadial(ref Vector3[] vertices, ref int i)
    {
        Vector3 direction = GetRightPos() - vertices[i];               // Direction from hand to vertice is given by subtracting one vector from the other
        vertices[i] += brushStrength * Time.deltaTime * -direction;    // Add the magnitude of push (brushStrength) * inverse of direction i.e. away from hand * 1/Framerate (for smooth transformation)
        _meshGenerator.selectors[i].transform.position = vertices[i];  // Move the selector spheres to the location of the new vertices
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PinchRadial(ref Vector3[] vertices, ref int i)
    {
        Vector3 direction = GetRightPos() - vertices[i];
        vertices[i] += brushStrength * Time.deltaTime * direction;        // Same but with absolute value of direction i.e. towards the hand
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }



    /// <summary>
    /// Y-axis Push and Pull Brushes - Only executed if hand is above vertice in question
    /// </summary>

    private void B_Level(ref Vector3[] vertices, ref int i)
    {
        float direction = GetRightPos().y - vertices[i].y;
        vertices[i].y += brushStrength * Time.deltaTime * direction;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
    }

    /// <summary>
    /// Same Brushes but vertices get pushed a magnitude inversly proportional from their distance to the hands
    /// </summary>
    private void B_PushDown(ref Vector3[] vertices, ref int i)
    {

        float distance = GetDistanceMagnitude(GetRightPos(), vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.down) / distance;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PullUp(ref Vector3[] vertices, ref int i)
    {
        float distance = GetDistanceMagnitude(GetRightPos(), vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.up) / distance;    // Multiplied by Vectro3.up i.e. (0,1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    // -------------
    private void B_PushDownVelocity(ref Vector3[] vertices, ref int i)
    {

        float distance = GetDistanceMagnitude(GetRightPos(), vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.down) + GetRightVel() / distance;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PullUpVelocity(ref Vector3[] vertices, ref int i)
    {
        float distance = GetDistanceMagnitude(GetRightPos(), vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.up) + GetRightVel() / distance;    // Multiplied by Vectro3.up i.e. (0,1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }
}
