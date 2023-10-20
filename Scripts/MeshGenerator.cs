using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]   // This makes it require a mesh filter just so a mesh is not added to nothing. Unity will crash if this happens, most likely.
public class MeshGenerator : MonoBehaviour
{   

    // VARIABLES -----------------------------------------------------------------------------------------------------------------------------------------------------------------
    Mesh mesh;               // Unity native 'mesh' object - picked up by Mesh Filter and , consequently, Mesh Renderer scripts that are also on the mesh Game Object in Unity

    // Arrays to hold vertices, triangles and colours ---------------------
    public Vector3[] vertices;     // Vector3 = (x,y,z) for vertices in the scene  
    public int[] triangles;        // int = (n) for triangles. Used as indices basically with three vertices indicating the position of the triangle.
    public Color[] colours;        // Unity native colour object defined in RGB. Used with Colour Gradient on Unity object by reference.

    // For grid of quads (made up of triangles made up of vertices) -------
    public int xSize = 20;     // Indicates total number of vertices in a row. Number of Quads in the row = (xSize - 1)
    public int zSize = 20;     // No. of vertices in a column
    public float verticeScale = 1.0f;
    public float verticeRadius = 2.0f;

    public GameObject[] selectors;

    // Perlin Noise variables --------------------------------------------- 
    [Header("Noise")]
    [SerializeField]private bool isPerlinNoise = true;
    [SerializeField]private List<NoiseLayer> noiseLayers = new();

    /*#if UNITY_EDITOR

        // This method updates / generates new plane.
        // Called from editor script on change.

        public void UpdatePlane()
        {
            CreateShape();
            UpdateMesh();
        }

    #endif*/

    // Colour Variables ---------------------------------------------------
    public Gradient gradient;  // Unity object has reference to editable gradient

    [HideInInspector] public float minHeight;           // To set the gradient according to min and max as it takes minimum and maximum value arguments
    [HideInInspector] public float maxHeight;





    // FUNCTIONS ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Start() ---------------------------------------------------------------------
    
    void Start()
    {
        mesh = new Mesh();   // Create new mesh object
        GetComponent<MeshFilter>().mesh = mesh;    // Indicate to the MeshFilter that its mesh component is the mesh we create here
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //mesh.GetComponent<Collider>().enabled = true;

        CreateShape();    // Creates the shape
        UpdateMesh();     // Updates it on the beginning frame

    }

    // CreateShape() ---------------------------------------------------------------
    /// <summary>
    /// For creating the vertices, triangles, uvs, and colours of the mesh plane for the first time.
    /// </summary>
    void CreateShape()
    {

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];   // Create a new Vector3 array the size of the indicated grid. (xSize + 1) because it takes (n + 1) vertices to create a grid with n quads per row/column
        selectors = new GameObject[(xSize + 1) * (zSize + 1)];  // Selectors array same size as vertices array
        
        // Creating vertices ------------------------------
        for (int z = 0, i = 0; z <= zSize; z++)    // Nested for-loops to go through all rows and columns
        { 
            for (int x = 0; x <= xSize; x++)
            {
                
                float y = 0;
                vertices[i] = (new Vector3(x, y, z) - new Vector3(xSize / 2f, 0, zSize / 2f)) * verticeScale;
                // Calculating elevation from noise layers.
                if (isPerlinNoise)
                {
                    float elevation = 0;
                    for (int l = 0; l < noiseLayers.Count; l++)
                    {
                        elevation += noiseLayers[l].Elevate(x / (float)xSize, z / (float)zSize);   // Adding layers of Perlin Noise according to number of layers added in Inspector
                    }
                    
                    vertices[i].y = elevation * verticeScale;        // Adding elevation from perlin noise.
                }

                // Recording minHeight and maxHeight for Gradient
                if (vertices[i].y > maxHeight) maxHeight = vertices[i].y;      // Update minHeight and maxHeight according to y generated using Perlin Noise
                if (vertices[i].y < minHeight) minHeight = vertices[i].y;      // At the end, we should have a variable measure of the max height and the min height in the generation.

                // Creating Selector spheres at each vertice
                selectors[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                selectors[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * verticeRadius;
                selectors[i].transform.position = (new Vector3(x, vertices[i].y / verticeScale, z) - new Vector3(xSize / 2f, 0, zSize / 2f)) * verticeScale;  // Applying same transform as vertices[i] before nested for loops
                selectors[i].GetComponent<Collider>().enabled = false;    // Disable the colliders of the collision spheres

                i++;   // Increment i inside the row loop, after every run
            }   
        }


        // Creating Triangles --------------------------
        triangles = new int[xSize * zSize * 6];       // New array for storing all triangles. Total number of triangles = no. of quads desired * 6 (no. of vertices for each quad). Two triangles per quad.
        
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)   // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;                // Six vertices need to be defined for every triangle. Every six subsequent int values in the triangles array indicate one full quad with both triangles drawn clockwise
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

            }

            vert++;  // Skip generating a triangle from one row to another

        }

        // Colouring triangles ---------------------------
        colours = new Color[vertices.Length];

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                //float height = vertices[i].y / 0.5f;
                float height = Mathf.Abs( 1 - Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y));   // Lerp between maximum and minimum height and give value between 0 and 1
                //Debug.Log("height: " + height + " " + minHeight + " " + maxHeight);
                colours[i] = gradient.Evaluate(height);   
                i++;
            }
        }

    }

    // UpdateMesh() --------------------------------------------------------------------
    /// <summary>
    /// For updating the mesh objects vertices on every frame
    /// </summary>
    void UpdateMesh()
    {
        mesh.Clear();                  // Clear after previous frame

        mesh.vertices = vertices;       // Mesh creation in Unity takes these elements to be defined
        mesh.triangles = triangles;
        mesh.colors = colours; 

        mesh.RecalculateNormals();  // Unity calculates normals for all surfaces for lighting

        

    }

    // Update is called once per frame
    void Update()
    {

        UpdateMesh();     // Updates it every frame
        //Debug.Log(selectors[45].transform.position); // Way to reference the selctor sphere's position
        //Debug.Log(Time.deltaTime);
    }
}
