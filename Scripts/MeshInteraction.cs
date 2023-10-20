using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class MeshInteraction : MonoBehaviour
{
    // ------------------------------------------------------------------------------- VARIABLES ---------------------------------------------------------------------------------------------//
    private InputData _inputData;                      // Reference in Start()
    private CharacterController _characterController;  // Reference in Start()
    public GameObject _player;                         // Reference given in Unity Inspector
    private MeshGenerator _meshGenerator;              // Reference in Start()


    [Header("Brush Settings")]
    public float brushRadius = 0.3f;              // Brush's Radius
    public Material brushInfluenceMaterial;       // Reference given in Unity Inspector
    private GameObject brushInfluence;

    public Material selectorsUnselected;          // Reference given in Unity Inspector
    public Material selectorsSelected;            // Reference given in Unity Inspector


    // -------------------------------------------------------------------------------- START() ---------------------------------------------------------------------------------------------//

    private void Start()
    {   
        _meshGenerator = GetComponent<MeshGenerator>();    // Get the MeshGenerator script attached to this object
        _inputData = GetComponent<InputData>();            // Get the InputData script attached to this object
        _characterController = _player.GetComponent<CharacterController>();   // Get the CharacterController script attached to the VR Origin object in the scene

        // Setup sphere around right hand which shows the sphere of influence of the brush in use ------------------------------------------------
        brushInfluence = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        brushInfluence.GetComponent<Renderer>().material = brushInfluenceMaterial;
        brushInfluence.GetComponent<Collider>().enabled = false;
        brushInfluence.transform.localScale = new Vector3(brushRadius, brushRadius, brushRadius);
        brushInfluence.transform.position = GetRightPos();

    }
    // ------------------------------------------------------------------------------- FUNCTIONS ---------------------------------------------------------------------------------------------//

    Vector3 GetCharPos()
    {
        return _characterController.transform.position; 
    }

    Vector3 GetRightPos()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPos)) return _characterController.transform.position + rightPos;
        else return new Vector3(0, 0, 0); 
    }

    Quaternion GetRightRot()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRot)) return rightRot;
        else return new Quaternion(1,1,1,1);
    }

    Vector3 GetLeftPos()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPos)) return _characterController.transform.position + leftPos;
        else return new Vector3(0, 0, 0);
    }

    Quaternion GetLeftRot()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRot)) return leftRot;
        else return new Quaternion(1, 1, 1, 1);
    }

    bool GetRightGrip()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.grip, out float gripVal))
        {
            if (gripVal > 0.6f) return true;
            else return false;
        }
        else return false;
    }

    bool GetRightTrigger()
    {
        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.trigger, out float trigVal))
        {
            if (trigVal > 0.6f) return true;
            else return false;
        }
        else return false;
    }

    // ------------------------------------------------------------------------------- UPDATE() ---------------------------------------------------------------------------------------------//
    void Update()
    {

        //Debug.Log(GetRightPos());
        if (GetRightGrip()) Debug.Log("Gripped");
        if (GetRightTrigger()) Debug.Log("Triggered");
        //Debug.Log(_meshGenerator.vertices[3]);
        

        Vector3[] oldVerts = _meshGenerator.vertices;
        Vector3[] newVerts = oldVerts;

        for (int z = 0, i = 0; z <= _meshGenerator.zSize; z++)    // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x <= _meshGenerator.xSize; x++)
            {
                if (Vector3.Distance(oldVerts[i], GetRightPos()) <= brushRadius)
                {
                    //newVerts[i] = new Vector3()
                    //Debug.Log("Vertice at " + oldVerts[i].x + ", " + oldVerts[i].y + ", " + oldVerts[i].z);
                    _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.8f);

                    if (GetRightTrigger()) 
                    { 
                        Vector3 direction = GetRightPos() - oldVerts[i];
                        oldVerts[i] -= direction * 3.0f;

                    }
                }
                else _meshGenerator.selectors[i].GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
                //Debug.Log(Vector3.Distance(_meshGenerator.vertices[i], GetRightPos()));

                i++;
            }
        }

        brushInfluence.transform.position = GetRightPos();    // KEEP IN UPDATE() - To move sphere of influence being rendered in the scene
        brushInfluence.transform.rotation = GetRightRot();    // KEEP IN UPDATE() - To move sphere of influence being rendered in the scene

    }
}
