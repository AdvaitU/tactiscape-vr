/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- IMPORT-EXPORT SCRIPT ------------------------------------------

- SUMMARY: This script deals with exporting the created and edited meshes as FBX files or PNG Depth Maps (Overhangs not supported in PNG)
- USED IN: (MeshInteractor script) Called in the main Update() loop when LeftSecondaryButton is pressed
- FOUND ON: 'Import/Export System' Game Object in Unity

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

using Autodesk.Fbx;
using UnityEditor;
using System.IO;
using UnityEditor.Formats.Fbx.Exporter;


// -------------------------------------------------------------------------------------------------------------------------------------------------------------START OF SCRIPT //
public class Import_Export : MonoBehaviour
{

    // OBJECT REFERENCES ----------------------------------------------------------------------------------------

    public GameObject _meshObject;  // Reference in Unity Inspector - Unity Game Object on which the mesh can be found
    private MeshGenerator _mesh;    // Object of type MeshGenerator

    // PUBLIC MEMBERS -------------------------------------------------------------------------------------------

    [Tooltip("If checked, the file will be exported as FBX file. Otherwise, it will be exported as a PNG Depth Map")]
    public bool isFBX = true;   // isFBX = true --> Exported as FBX file, isFBX = false --> Exported as PNG file

    [Header("PNG Export Settings")]
    
    [Tooltip("Factor by which PNG Depth Map Export is scaled up from (grid width x grid height) pixels. \n Eg: If the Landscape is 100x100 vertices and PNG Res Scale is 5 (default), the Depth Map will be 500x500 pixels")]
    [Range(1, 10)]
    public int resScale = 5;   // Factor by which PNG Depth Map Export is scaled up from (grid width x grid height) pixels.

    [Tooltip("If checked, PNG will be in colour shaded according to the gradient of the actual landscape. If unchecked, it will be a B&W depth Map that is easier to use in other programmes.")]
    public bool isColour = true;   // f checked, PNG will be in colour shaded according to the gradient of the actual landscape.

    // PRIVATE MEMBERS ------------------------------------------------------------------------------------------

    private Texture2D _texture;    // Creating the texture to be used by the class
    private int exportCounter = 1; // To name multiple exports in the same session according to a counter



    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Start() : Get the MeshGenerator script attached to the Mesh Renderer object in the scene.

        // PNG METHODS ---------------------
        - CreateTexture() : Creates Texture2D object from vertices in _mesh from MeshGenerator script. Sets colour according to MeshGenerator gradient (Reference in Unity Inspector) or B&W
                          : Instantiates and returns a Texture2D in colour or Black and White based on if isColour is ticked.
        - ScaleTexture() : Takes a Texture2D and scales it up according to the multiplier bi-linearly. 
                          : Eg: If scaled up by 5, function will take original image, multiply size by 5
                                and Lerp between subsequeent pixels to fill in the other 4 pixels.
                          : Technique and code courtesy 'Eric5h5'. Find at: https://web.archive.org/web/20210506234020/http://wiki.unity3d.com/index.php/TextureScale
        - SaveToPNG() : Saves created (and potentially scaled) texture to a PNG file in the format: "Export_n_on_DD-MM-YYYY.png" in directory "./Assets/PNG_Exports/"
                      : Takes Texture2D as input parameter.
        - ExportToPNG() : Cumulative function that combines all three methods in the class in one - for easy use in other scripts.
                        : Called from MeshInteractor script when Left Controller secondary button is pushed.

        // FBX METHODS ---------------------

        - ExportToFBX() : Exports the scene as an FBX so it can be imported into other applications.

        // CUMULATIVE ----------------------
        - *** ExportScene() *** : Takes the _mesh as an argument. Used in Update() of MeshInteractor script.
                                : Exports the scene as either an FBX or a coloured or B&W PNG Depth Map according to the user's choice.

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // ------------------------------------------------------------------------------------------
    void Start()
    {
        _mesh = _meshObject.GetComponent<MeshGenerator>();   // Get the MeshGenerator script attached to the Mesh Renderer object in the scene
    }


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




    private Texture2D ScaleTexture(Texture2D texture)
    {
        TextureScale.Bilinear(texture, texture.width * resScale, texture.height * resScale);
        return texture; 
    }




    // Code cumulated based on answers to 'Therazerproject's original question on the Unity Forums: https://discussions.unity.com/t/how-to-save-a-texture2d-into-a-png/184699
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


    // -- CUMULATIVE ---------------------------------------------------------------------------
    public void ExportToPNG()
    {
        _texture = CreateTexture();
        ScaleTexture(_texture);
        SaveToPNG(_texture);
    }

    // This method courtesy Unity Official Documentation at https://docs.unity3d.com/Packages/com.unity.formats.fbx@2.0/manual/devguide.html
    public void ExportTOFBX(Object objects)
    {
        string filePath = Application.dataPath + "/Exports/FBX/FBX_" + exportCounter + "_on_" + System.DateTime.Now.ToString()[0..10] + ".fbx";
        ModelExporter.ExportObject(filePath, objects);
        Debug.Log("FBX Export saved to " + filePath);
    }


    // *** -- CUMULATIVE ---------------- *** ------------------------------ *** ---- ExportScene() METHOD ---- *** ------------------------------ *** -------------------------- //

    public void ExportScene(MeshGenerator _mesh)
    {
        if (isFBX) ExportTOFBX(_mesh);
        else ExportToPNG();
    }

    // *** ------------------------------ *** ------------------------------ *** ------------------------------ *** ------------------------------ *** -------------------------- //
}





// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //