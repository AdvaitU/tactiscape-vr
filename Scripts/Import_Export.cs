using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

using Autodesk.Fbx;
using UnityEditor;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;


public class Import_Export : MonoBehaviour
{
    public GameObject _meshObject;  // Unity Game Object on which the mesh can be found
    private MeshGenerator _mesh;    // Object of type MeshGenerator

    private Texture2D _texture;    // Creating the texture to be used by the class
    private int exportCounter = 1; // To name multiple exports in the same session according to a counter

    [Tooltip("If checked, the file will be exported as FBX file. Otherwise, it will be exported as a PNG Depth Map")]
    public bool isFBX = true;   // isFBX = true --> Exported as FBX file, isFBX = false --> Exported as PNG file

    [Header("PNG Export Settings")]
    [Tooltip("Factor by which PNG Depth Map Export is scaled up from (grid width x grid height) pixels. \n Eg: If the Landscape is 100x100 vertices and PNG Res Scale is 5 (default), the Depth Map will be 500x500 pixels")]
    [Range(1, 10)]
    public int resScale = 5;
    [Tooltip("If checked, PNG will be in colour shaded according to the gradient of the actual landscape. If unchecked, it will be a B&W depth Map that is easier to use in other programmes.")]
    public bool isColour = true;

    // ------------------------------------------------------------------------------- START ------------------------------------------------------------------------------------------------//
    void Start()
    {
        _mesh = _meshObject.GetComponent<MeshGenerator>();   // Get the MeshGenerator script attached to the Mesh Renderer object in the scene
    }

    // --------------------------------------------------------------------------- FUNCTIONS ------------------------------------------------------------------------------------------------//
    
    /// <summary> ---------------------------------------------------------------------------------------------------------------------------------
    /// Creates Texture from vertices in MeshGenerator script
    /// </summary>
    /// <param name="isColour"> True = Colour Depth Map according to colours in Vertex Shader; False = Black and White Depth Map</param>
    /// <returns> texture as Texture2D object</returns>
    private Texture2D CreateTexture()  // Defaults to coloured image
    {
        Texture2D texture = new(_mesh.xSize, _mesh.zSize);               // Create a new texture the same size as the mesh for colour export

        for (int z = 0, i = 0; z <= texture.width; z++)        // Nested for-loops to go through all rows and columns
        {
            for (int x = 0; x <= texture.height; x++)
            {
                float magnitude = Mathf.Abs(1 - Mathf.InverseLerp(_mesh.minHeight, _mesh.maxHeight, _mesh.vertices[i].y));  // Get the vertice's y co-ordinate and normalise to float between 0-1
                
                Color color;   // Initialise a color
                if (isColour) color = _mesh.gradient.Evaluate(magnitude);                        // Evaluate using same gradient that shades terrain
                else color = Color.white * new Vector4(magnitude, magnitude, magnitude, 1.0f);   // Or set as grey with whiteness proportional to magnitude
                
                texture.SetPixel(x, z, color);                      // Set the colour according to gradient

                i++;   // Increment i
            }

        }
        return texture;   // Return the local texture.
    }

    /// <summary> ---------------------------------------------------------------------------------------------------------------------------------
    /// Takes a Texture2D and scales it up according to the multiplier bi-linearly. If scaled up by 5 for example, function will take original image, multiply size by 5
    /// and Lerp between subsequeent pixels to fill in the other 4 pixels
    /// </summary>
    /// <param name="texture"> Texture from CreateTexture return </param>
    /// <param name="scale"> Int multiplier to scale up image from resolution of mesh </param>
    /// <returns></returns>
    private Texture2D ScaleTexture(Texture2D texture)
    {
        TextureScale.Bilinear(texture, texture.width * resScale, texture.height * resScale);
        return texture; 
    }

    /// <summary> ---------------------------------------------------------------------------------------------------------------------------------
    /// Saves created (and potentially scaled) texture to a PNG file in the format: "Export_n_on_DD-MM-YYYY.png" in directory "./Assets/PNG_Exports/"
    /// </summary>
    /// <param name="texture"> Texture2D object returned from CreateTexture() or ScaleTexture()</param>
    private void SaveToPNG(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();                    // Encode to PNG as byte array
        var dirPath = Application.dataPath + "/Exports/PNG/";     // Set directory path - Append "/PNG_Exports" to current directory
        if (!System.IO.Directory.Exists(dirPath))                // If Directory doesn't exist
        {
            System.IO.Directory.CreateDirectory(dirPath);        // Make it
        }
        string fileName = dirPath + "PNG_" + exportCounter + "_on_" + System.DateTime.Now.ToString()[0..10] + ".png";  // Create File Name
        System.IO.File.WriteAllBytes(fileName, bytes);   // Write file
        exportCounter++;     // Increase counter to differentiate exports from the same session
        
        Debug.Log(fileName + "(" + bytes.Length / 1024 + "Kb) saved to " + dirPath);   // Console Log

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

    }

    // ------------------------------------------------------------------------ CUMULATIVE FUNCTION ----------------------------------------------------------------------------------------//

    /// <summary> ---------------------------------------------------------------------------------------------------------------------------------
    /// Cumulative function that combines all three methods in the class in one - for easy use in other scripts
    /// Called from MeshInteractor script when Left Controller secondary button is pushed
    /// </summary>
    /// <param name="isColour"> Same as CreateTexture() </param>
    /// <param name="resScale"> Same as ScaleTexture() </param>

    public void ExportToPNG()
    {
        _texture = CreateTexture();
        ScaleTexture(_texture);
        SaveToPNG(_texture);
    }


    // ---------------------------------------------------------------------- EXPORT GAMEOBJECT METHOD ---------------------------------------------------------------------------//
    // This and the following two FBX methods courtesy Unity Official Documentation at https://docs.unity3d.com/Packages/com.unity.formats.fbx@2.0/manual/devguide.html
    public void ExportGameObject(Object objects)
    {
        string filePath = Application.dataPath + "/Exports/FBX/FBX_" + exportCounter + "_on_" + System.DateTime.Now.ToString()[0..10] + ".fbx";
        ModelExporter.ExportObject(filePath, objects);
        Debug.Log("FBX Export saved to " + filePath);

        // ModelExporter.ExportObject can be used instead of 
        // ModelExporter.ExportObjects to export a single game object
    }


    public void ImportGameObject(Object objects)
    {
        string filePath = Application.dataPath + "/Exports/FBX/FBX_" + exportCounter + "_on_" + System.DateTime.Now.ToString()[0..10] + ".fbx";
        ModelExporter.ExportObject(filePath, objects);
        Debug.Log("FBX Export saved to " + filePath);

        // ModelExporter.ExportObject can be used instead of 
        // ModelExporter.ExportObjects to export a single game object
    }


    // ---------------------------------------------------------------------------- EXPORT AS FBX -------------------------------------------------------------------------------//
    /// <summary>
    /// Export the file as FBX instead of PNG depth map so it can be imported into other applications.
    /// </summary>
    /// <param name="fileName"></param>
    public void ExportAsFBX(string fileName)
    {
        using FbxManager fbxManager = FbxManager.Create();
        fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT));  // configure IO settings.
        using FbxExporter exporter = FbxExporter.Create(fbxManager, "Exporter");   // Export the scene
        bool status = exporter.Initialize(fileName, -1, fbxManager.GetIOSettings());   // Initialize the exporter.
        FbxScene scene = FbxScene.Create(fbxManager, "sceneToExport");  // Create a new scene to export
        exporter.Export(scene);    // Export the scene to the file.
        Debug.Log("Scene exported as FBX file");
    }

    // --------------------------------------------------------------------------- IMPORT FROM FBX ------------------------------------------------------------------------------//

    public void ImportScene(string fileName)
    {
        using FbxManager fbxManager = FbxManager.Create();
        fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT)); // configure IO settings.
        using FbxImporter importer = FbxImporter.Create(fbxManager, "Importer"); // Import the scene to make sure file is valid
        bool status = importer.Initialize(fileName, -1, fbxManager.GetIOSettings());  // Initialize the importer.
        FbxScene scene = FbxScene.Create(fbxManager, "importedScene");  // Create a new scene so it can be populated by the imported file.
        importer.Import(scene);  // Import the contents of the file into the scene.
    }


    // ------------------------------------------------------------------------------- UPDATE ------------------------------------------------------------------------------------------------//
    // Update empty
    void Update()
    {
        //Debug.Log(_mesh.vertices[3]);
        /*if (doOnce)
        {
            _texture = CreateTexture();
            ScaleTexture(_texture);
            SaveToPNG(_texture);
            doOnce = false;
        }*/

    }
}
