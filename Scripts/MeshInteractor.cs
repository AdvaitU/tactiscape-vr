/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                -------------------------------------- MESH INTERACTOR (MAIN SCRIPT) ------------------------------------------

- SUMMARY: This script allows for interaction with the mesh to deform it using multiple brush shapes, sizes, and impact types. It also instantiates and calls 
           all the other components of TactiScape in its Update Function
- USED IN: Is the one that uses.
- FOUND ON: 'Mesh Renderer' Game Object in Unity.

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(InputData))]             // Requires Input Data Script to work i.e. if InputData doesn't detect anything, the application will be in 'View' Mode
public class MeshInteractor : MonoBehaviour
{
    // ------------------------------------------------------------------------------- VARIABLES ---------------------------------------------------------------------------------------------//


    // OBJECT REFERENCES - PUBLIC ----------------------------------------------------------------------------------------
    [Tooltip("Reference to Player Game Object - In this case, it would be VR Origin")]
    public GameObject _player;                                  // Reference given in Unity Inspector  - Reference to VR origin (Player)
    [Tooltip("Reference to Exporter Object - In this case, it would be Import_Export")]
    public GameObject _import_export;                           // Reference given in Unity Inspector - Import_Export Game Object
    [Tooltip("Forward Vector of Right Hand - Drag Transform from 'VR Origin -> Camera Offset -> Right Hand -> Mesh -> Transform' here")]
    public Transform _rightForward;                             // Reference given in Unity Inspector - Forward vector of Right Hand
    [Tooltip("Reference to Influence Indication script that enables and disables models that show the brush's area of impact")]
    public GameObject _influenceIndication;                     // Reference given in Unity Inspector - Translucent models that show the area of influence
    [Tooltip("Reference to Landscape Scaling script attached to Game Object")]
    


    // OBJECT REFERENCES - PRIVATE ----------------------------------------------------------------------------------------
    private InfluenceIndication _influence;                     // Reference in Start() - Instance of InfluenceIndication Script
    private Import_Export _exporter;                            // Reference in Start() - Instance of Import_Export Script
    private InputData _inputData;                               // Reference in Start() - Accessing inputData script which initialises and names HMD, right and left controllers
    private CharacterController _characterController;           // Reference in Start() - To access the position of the character ion the world space i.e. wrt vertices
    private MeshGenerator _meshGenerator;                       // Reference in Start() - Mesh Generation script which updates and stores info on vertices, triangles, uvs, and normals
    private HapticFeedback _hapticFeedback;                     // Reference in Start() - HapticFeedback script that contains methods to send Haptic Feedback to right or left controllers
    private ActionBasedContinuousMoveProvider _moveProvider;    // Reference in Start() - Move Providor on VR Origin object to set movement speed in scaled landscape
    private LandscapeScaling _landscapeScaling;                 // Reference in Start() - Landscape Scaling Script



    // BRUSH SETTINGS ----------------------------
    [Header("Brush Settings")]
    [Tooltip("How large the area of impact is")]
    [Range(0.0f, 3.0f)]
    public float brushRadius = 0.3f;              // Brush's Radius - For spherical basic brushy
    [Tooltip("How hard is the impact. It ideally scales with the brush size")]
    [Range(0.0f, 3.0f)]
    public float brushStrength = 1.0f;            // Magnitude with which vertices are pushed
    [Tooltip("Tick if you want the Strength to Scale with the Radius i.e. mimicing the real world")]
    public bool strengthProportionalToRadius = true;
    [Tooltip("'Slope' of the conical brush. Default 0.5 = 60 degrees slope. 1 = 90 degrees slop i.e. hemisphere. 0 = 0 degrees slope i.e. Cone practically doesn't exist")]
    public float coneFieldOfView = 0.5f;           // 'Field of View' of the cone brush in front of the user
    private readonly float brushRadiusMin = 0.1f;  // Minimum and Maxiumum permissible brush sizes in the programme
    private readonly float brushRadiusMax = 3.0f;
    // Selected Brush Shapes and impacts ---------
    [Tooltip("1. Free Form Deformation\n2. Velocity Up and Down\n3. Pinch Radial\n4. Push Radial\n5. Flatten")]
    [Range(1, 5)]
    public int selectedBrush = 3;
    [Tooltip("1. Sphere\n2. Hemisphere Down\n3. Hemisphere Up\n4. Cone Forward\n5. Cone Downward")]
    [Range(1, 5)]
    public int selectedShape = 5;

    // HAPTIC FEEDBACK ---------------------------
    private float currTime;
    private bool sendHapticFeedbackInteract = false;   // Starts as false. If true, Haptic Feedback is sent to the user
    private bool sendHapticFeedbackTouch = false;   // Starts as false. If true, Haptic Feedback is sent to the user

    



    // -------------------------------------------------------------------------------- START() ---------------------------------------------------------------------------------------------//

    private void Start()
    {
        // GetComponents for all scripts and requirements as necessary
        _meshGenerator = GetComponent<MeshGenerator>();    // Get the MeshGenerator script attached to this object
        _inputData = GetComponent<InputData>();            // Get the InputData script attached to this object
        _characterController = _player.GetComponent<CharacterController>();   // Get the CharacterController script attached to the VR Origin object in the scene
        _exporter = _import_export.GetComponent<Import_Export>();             // Get the Import_Export script from the Import_Export object
        _hapticFeedback = GetComponent<HapticFeedback>();                     // Get Haptic Feedback script
        _influence = _influenceIndication.GetComponent<InfluenceIndication>(); // Get Influence Indication Script
        _landscapeScaling = GetComponent<LandscapeScaling>();
        _moveProvider = _player.GetComponent<ActionBasedContinuousMoveProvider>();

        // Save time once on the first frame
        currTime = Time.time;

        // Set brush Strength equal to Radius by overriding Inspector if option is ticked
        if (strengthProportionalToRadius) brushStrength = brushRadius;
    }

    // ------------------------------------------------------------------------------- UPDATE() ---------------------------------------------------------------------------------------------//
    void Update()
    {

        // INTERACTIONS FOR BUTTONS ---------------------------------------------------------------------------------------------------------------
        // Description: These conditions watch out for button presses on every frame and execute the associated functionality


        // 1. EXPORTER
        if (LeftSecondaryButton) _exporter.ExportScene(_meshGenerator);
        
        // 2. LANDSCAPE SCALING
        if (LeftPrimaryButton) _landscapeScaling.ToggleScale(_meshGenerator, _meshGenerator.xSize, _meshGenerator.zSize, _moveProvider, _player);

        // 3. CHANGE BRUSH IMPACT STYLE
        if (RightGrip && !RightPrimaryButton)
        {
            selectedBrush++;
            if (selectedBrush > 5) selectedBrush = 1;  // Clip between 1 and 5
        }

        // 4. CHANGE BRUSH SHAPE
        if (LeftGrip && !RightPrimaryButton)
        {
            selectedShape++;
            if (selectedShape > 5) selectedShape = 1;  // Clip between 1 and 5
        }

        // 5. CHANGE BRUSH SIZE
        if (RightPrimaryButton)
        {
            if (RightGrip) brushRadius += 0.1f;
            if (LeftGrip) brushRadius -= 0.1f;

            if (brushRadius > brushRadiusMax) brushRadius = brushRadiusMin;      // Clip between 0.1 and 3.0
            else if (brushRadius < brushRadiusMin) brushRadius = brushRadiusMax;
        }


        // Haptic Feedback
        sendHapticFeedbackInteract = false; /* AND */ sendHapticFeedbackTouch = false;   // Set as false at the beginning of every frame

        // --------------------------------------------------------------------- NON AVERAGE BRUSHES ------------------------------------------------------------------------- //
        // Description: Regular function - Cumulative of all other methods in the entire programme

        for (int z = 0, i = 0; z <= _meshGenerator.zSize; z++)    // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x <= _meshGenerator.xSize; x++, i++)
            {
                M_ShapeToggle(ref _meshGenerator.vertices, ref i);

                if (_meshGenerator.vertices[i].y > _meshGenerator.maxHeight) _meshGenerator.maxHeight = _meshGenerator.vertices[i].y;      // Update minHeight and maxHeight according to y generated using Perlin Noise
                if (_meshGenerator.vertices[i].y < _meshGenerator.minHeight) _meshGenerator.minHeight = _meshGenerator.vertices[i].y;      // At the end, we should have a variable measure of the max height and the min height in the generation.
            }
        }


        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------ AVERAGE BRUSHES ----------------------------------------------------------------------------- //
        // Description: Commented. Uncomment only while using brushes that need to average the position of all vertices within impact range. Experimental feature, currently none 
        //              of the brushes use this. But it's tested and works.
        /*
                List<Vector3> relevantVertices = new();
                Vector3 aggregateVertices = Vector3.zero;


                for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
                {
                    for (int x = 0; x <= xSize; x++, i++)
                    {
                        // {-- Put Average Based Action Brushes here --}

                        if (newVertices[i].y > maxHeight) maxHeight = newVertices[i].y;      // Update minHeight and maxHeight according to y generated using Perlin Noise
                        if (newVertices[i].y < minHeight) minHeight = newVertices[i].y;      // At the end, we should have a variable measure of the max height and the min height in the generation.
                    }
                }

                relevantVertices.Clear();
        
         */

        // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- //



        // Haptic Feedback check
        if (!_hapticFeedback.hapticFeedbackOn) { /* Do nothing */ }
        else if (sendHapticFeedbackInteract) _hapticFeedback.SendHaptics(_hapticFeedback.interact, RightVelMagnitude);  // Interact has priority, send if interact is true
        else if (sendHapticFeedbackTouch) _hapticFeedback.SendHaptics(_hapticFeedback.touch);   // Else check touch and send touch
        else { /* Do nothing */ }


        // Reset colours of the vertex shader
        for (int z = 0, i = 0; z <= _meshGenerator.zSize; z++)
        {
            for (int x = 0; x <= _meshGenerator.xSize; x++)
            {
                float height = Mathf.Abs(1 - Mathf.InverseLerp(_meshGenerator.minHeight, _meshGenerator.maxHeight, _meshGenerator.vertices[i].y));   // Lerp between maximum and minimum height and give value between 0 and 1
                _meshGenerator.colours[i] = _meshGenerator.gradient.Evaluate(height);
                i++;
            }
        }

        // Proportionality check
        if (strengthProportionalToRadius) if (brushStrength != brushRadius) brushStrength = brushRadius;


    }

    // ------------------------------------------------------------------------ END OF MAIN PROGRAMME ---------------------------------------------------------------------------//






    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //
        The Following Section contains all the component methods used in the main frame-to-frame loop divided into 4 sections based on function:


        - 1. Getters and Checkers - These are actually properties, not methods.
                                    These check for Button Inputs, Hand and HMD Positions, etc.
        - 2. Calculators - Used for basic vector calculations where required
        - 3. Brush Shapes and Impacts - Main Section which defines all the brush behaviour.
        - 4.

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/



    // SECTION 1 --> GETTERS AND CHECKERS =====================================================================================================================================
    /*
     * All of these act as Getters or Checkers for various properties of the hand wrt to the world it exists in. For eg: The GetRightPos() method takes into account the distance of the 
     * Right Hand Controller from the HMD which is directly linked to the player's position in the world and adds the two Vector3s to give the position of the hand in the world.
     * 
     */

    // Position, Rotation, Velocity, etc. ------------------------------------------------------------------------
    // Character ----------------------------------------------------
    private Vector3 CharPos => _characterController.transform.position;

    // Right Hand ----------------------------------------------------
    private Vector3 RightPos
    {
        get
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos)) return CharPos + rightPos;
            else return new Vector3(0, 0, 0);
        }
    }

    private Vector3 RightVel
    {
        get
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVel)) return rightVel;
            else return new Vector3(0, 0, 0);
        }
    }

    private float RightVelMagnitude
    {
        get
        {
            Vector3 v = RightVel;
            return Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
    }

    Vector3 RightForwardVector => _rightForward.transform.forward;

    // Button Presses ---------------------------------------------------------------------------------------------------
    // Right Hand ----------------------------------------------------

    private bool RightTrigger
    {
        get
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out float trigVal))
            {
                if (trigVal > 0.6f) return true;
                else return false;
            }
            else return false;
        }
    }

    private bool RightGrip
    {
        get
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.grip, out float gripVal))
            {
                if (gripVal > 0.6f) return true;
                else return false;
            }
            else return false;
        }
    }

    private bool RightPrimaryButton
    {
        get
        {
            if (_inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPrimary) && isPrimary) return true;
            else return false;
        }
    }

    bool GetRightSecondaryButton()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isSecondary) && isSecondary) return true;
        else return false;
    }



    // Left Hand ----------------------------------------------------

    private bool LeftTrigger
    {
        get
        {
            if (_inputData._leftController.TryGetFeatureValue(CommonUsages.trigger, out float trigVal))
            {
                if (trigVal > 0.6f) return true;
                else return false;
            }
            else return false;
        }
    }

    private bool LeftGrip
    {
        get
        {
            if (_inputData._leftController.TryGetFeatureValue(CommonUsages.grip, out float gripVal))
            {
                if (gripVal > 0.6f) return true;
                else return false;
            }
            else return false;
        }
    }

    private bool LeftSecondaryButton
    {
        get
        {
            if (_inputData._leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isSecondary) && isSecondary) return true;
            else return false;
        }
    }

    private bool LeftPrimaryButton
    {
        get
        {
            if (_inputData._leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isPrimary) && isPrimary) return true;
            else return false;
        }
    }


    // ========================================================================================================================================================== END OF SECTION //

    // ------------------------------------------------------------------------------ CALCULATORS -------------------------------------------------------------------------------------------//
    private static float GetDistanceMagnitude(Vector3 a, Vector3 b)          // Get the Magnitue of distance between two vectors as a float
    {
        Vector3 vector = new(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }

    private static bool AnyVelocityComponentHigherThan(Vector3 a, float margin)       // Checks if there is velocity in any direction
    {
        if (a.x > margin || a.y > margin || a.z > margin || a.x < -margin || a.y < -margin || a.z < -margin)   // If any of the 3 components are higher than the positive margin or lower than the negative margin i.e. movement above margin value in positive or negative direction
        {
            return true;   // Assume the hand has moved and return true to call haptics
        }
        else return false;  // Else assume the hand hasn't moved. margin is necessary as a limiter as the hand shakes a little anyway and we don't want the haptics to go off in that case
    }


    // ------------------------------------------------------------------------------- BRUSH SHAPES ---------------------------------------------------------------------------------------------//
    // Description: These act as Brush Shape Setters, 'selecting' a few of the vertices in the mesh for interaction

    // Final Brush Shapes -----------------------------------------------------------------------------
    /* 1. Sphere
     * 2. Hemisphere Down
     * 3. Hemisphere Up
     * 4. Cone Forward (Flashlight)
     * 5. Cone Downward (Stamp)
     * 6. Hemisphere Forward
     * 7. Hemisphere Backward
     */
    private bool SH_Sphere(ref Vector3[] vertices, ref int i)
    {
        _influence.SetActiveBrush(2, brushRadius);
        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius)     // Simple Distance check around right hand
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }
    }

    private bool SH_HemisphereUp(ref Vector3[] vertices, ref int i)
    {
        _influence.SetActiveBrush(3, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (RightPos.y <= vertices[i].y))    // Distance + height comparison
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }
    }

    private bool SH_HemisphereDown(ref Vector3[] vertices, ref int i)
    {
        _influence.SetActiveBrush(3, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (RightPos.y >= vertices[i].y))
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }
    }

    private bool SH_ConeForward(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = RightForwardVector;
        Vector3 directiontoVertice = (RightPos - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);

        _influence.SetActiveBrush(0, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (dotProductOfVectors > coneFieldOfView)) // To point it forward Since cos(0) = 1
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }

    private bool SH_ConeDownward(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = RightForwardVector;
        Vector3 directiontoVertice = (RightPos - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);

        _influence.SetActiveBrush(1, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (dotProductOfVectors < coneFieldOfView/2 || dotProductOfVectors > -coneFieldOfView / 2))  // To point it downwards Since cos(90) = 0
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }

    // Legacy Brush Shapes ------------------------
    private bool SH_HemisphereForward(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = RightForwardVector;
        Vector3 directiontoVertice = (RightPos - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);

        _influence.SetActiveBrush(0, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (dotProductOfVectors > 0)) // To point it forward Since cos(0) = 1
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }

    private bool SH_HemisphereBackward(ref Vector3[] vertices, ref int i)
    {
        Vector3 frontDirection = RightForwardVector;
        Vector3 directiontoVertice = (RightPos - vertices[i]).normalized;
        float dotProductOfVectors = -Vector3.Dot(directiontoVertice, frontDirection);

        _influence.SetActiveBrush(0, brushRadius);

        if (Vector3.Distance(vertices[i], RightPos) <= brushRadius && (dotProductOfVectors < 0)) // To point it downwards Since cos(0) = 1
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.8f);      // Colour it red
            if (AnyVelocityComponentHigherThan(RightVel, 0.05f)) sendHapticFeedbackTouch = true;   // Touched is set to true
            return true;
        }
        else
        {
            _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new UnityEngine.Color(1.0f, 1.0f, 1.0f, 0.1f);     // Else colour it white
            return false;
        }

    }


    // ------------------------------------------------------------------------------- BRUSHES ---------------------------------------------------------------------------------------------//

    // Final Brushes -----------------------------------------------------------------------------
    /* 1. Free Form Deformation - Velocity Brush
     * 2. Velocity Up and Down
     * 3. Pinch Radial
     * 4. Push Radial
     * 5. Flatten
     */

    private void B_FreeForm(ref Vector3[] vertices, ref int i)
    {
        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += brushStrength * Time.deltaTime * RightVel / distance;
        _meshGenerator.selectors[i].transform.position = vertices[i];  // Move the selector spheres to the location of the new vertices
    }

    private void B_UpDownVelocity(ref Vector3[] vertices, ref int i)       // According to velocity but ony move in y
    {

        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += brushStrength * RightVel.y * Time.deltaTime * Vector3.up / distance;    // Multiplied by Vectro3.up i.e. (0,1,0) to cancel out its effects along x and z axes
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PinchRadial(ref Vector3[] vertices, ref int i)
    {
        Vector3 direction = RightPos - vertices[i];
        vertices[i] += brushStrength * Time.deltaTime * direction;        // Same but with absolute value of direction i.e. towards the hand
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PushRadial(ref Vector3[] vertices, ref int i)
    {
        Vector3 direction = RightPos - vertices[i];               // Direction from hand to vertice is given by subtracting one vector from the other
        vertices[i] += brushStrength * Time.deltaTime * -direction;    // Add the magnitude of push (brushStrength) * inverse of direction i.e. away from hand * 1/Framerate (for smooth transformation)
        _meshGenerator.selectors[i].transform.position = vertices[i];  // Move the selector spheres to the location of the new vertices
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_Flatten(ref Vector3[] vertices, ref int i)
    {
        float direction = RightPos.y - vertices[i].y;
        vertices[i].y += brushStrength * Time.deltaTime * direction;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
    }

    // Legacy Brushes -----------------------------------------------------------------------------
    /* 1. Push Down - Constant Force
     * 2. Pull Up - Constant Force
     * 3. Push Down - Velocity in Y
     * 4. Pull Up - Velocity in Y
     */

    private void B_PushDown(ref Vector3[] vertices, ref int i)
    {

        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.down) / distance;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PullUp(ref Vector3[] vertices, ref int i)
    {
        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.up) / distance;    // Multiplied by Vectro3.up i.e. (0,1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PushDownVelocity(ref Vector3[] vertices, ref int i)
    {

        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += brushStrength * Time.deltaTime * Vector3.down * RightVelMagnitude / distance;    // Multiplied by Vectro3.down i.e. (0,-1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    private void B_PullUpVelocity(ref Vector3[] vertices, ref int i)
    {
        float distance = GetDistanceMagnitude(RightPos, vertices[i]);
        vertices[i] += (brushStrength * Time.deltaTime * Vector3.up) + RightVel / distance;    // Multiplied by Vectro3.up i.e. (0,1,0)
        _meshGenerator.selectors[i].transform.position = vertices[i];
        sendHapticFeedbackInteract = true;  // Set interact as true but touch as false if interacted
    }

    // BRUSH TOGGLES AND SWITCH -----------------------------------------------------------------------------------------------------------------------------------------------

    private void M_ImpactToggle(ref Vector3[] vertices, ref int i)
    {
        if (RightTrigger && selectedBrush <= 5)   // If right trigger is pressed
        {
            switch (selectedBrush)
            {
                case 1:
                    B_FreeForm(ref vertices, ref i);
                    break;
                case 2:
                    B_UpDownVelocity(ref vertices, ref i);
                    break;
                case 3:
                    B_PinchRadial(ref vertices, ref i);
                    break;
                case 4:
                    B_PushRadial(ref vertices, ref i);
                    break;
                case 5:
                    B_Flatten(ref vertices, ref i);
                    break;
                default:
                    break;
            }
        }
    }

    private void M_ShapeToggle(ref Vector3[] vertices, ref int i)
    {
        switch (selectedShape)
        {
            case 1:
                if (SH_Sphere(ref vertices, ref i) && selectedShape <= 5) M_ImpactToggle(ref vertices, ref i);
                break;
            case 2:
                if (SH_HemisphereDown(ref vertices, ref i) && selectedShape <= 5) M_ImpactToggle(ref vertices, ref i);
                break;
            case 3:
                if (SH_HemisphereUp(ref vertices, ref i) && selectedShape <= 5) M_ImpactToggle(ref vertices, ref i);
                break;
            case 4:
                if (SH_ConeForward(ref vertices, ref i) && selectedShape <= 5) M_ImpactToggle(ref vertices, ref i);
                break;
            case 5:
                if (SH_ConeDownward(ref vertices, ref i) && selectedShape <= 5) M_ImpactToggle(ref vertices, ref i);
                break;
            default:
                break;

        }
    }
}
