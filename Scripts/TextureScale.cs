/* ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
                                      -------------------------------------- TEXTURE SCALE SCRIPT ------------------------------------------

- SUMMARY: This script scales the a Texture2D object by an input factor
- USED IN: (Import_Export.cs) Called in the main Import_Export.ScaleTexture() and Import_Export.ExportToPNG()
- FOUND ON: 'Import/Export System' Game Object in Unity
- CREDITS: Thanks to Eric5h5: https://web.archive.org/web/20210506234020/http://wiki.unity3d.com/index.php/TextureScale

// ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

// Namespaces -------------------------------------------------------------------
using System.Threading;
using UnityEngine;


// -------------------------------------------------------------------------------------------------------------------------------------------------------------START OF SCRIPT //
public class TextureScale : MonoBehaviour
{   
    // CONSTRUCTOR ----------------------------------------------------------------
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    // PRIVATE MEMBERS ------------------------------------------------------------------------------------------
    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;

    /* ------------------------------------------------------------------------- METHODS ---------------------------------------------------------------------------------- //

        - Start() : Get the MeshGenerator script attached to the Mesh Renderer object in the scene.

        // PNG METHODS ---------------------
        - Point() : Creates Texture2D object from vertices in _mesh from MeshGenerator script. Sets colour according to MeshGenerator gradient (Reference in Unity Inspector) or B&W
                          : Instantiates and returns a Texture2D in colour or Black and White based on if isColour is ticked.
        - Bilinear() : Takes a Texture2D and scales it up according to the multiplier bi-linearly. 
                          : Eg: If scaled up by 5, function will take original image, multiply size by 5
                                and Lerp between subsequeent pixels to fill in the other 4 pixels.
                          : Technique and code courtesy 'Eric5h5'. Find at: https://web.archive.org/web/20210506234020/http://wiki.unity3d.com/index.php/TextureScale
        - ThreadedScale) : Saves created (and potentially scaled) texture to a PNG file in the format: "Export_n_on_DD-MM-YYYY.png" in directory "./Assets/PNG_Exports/"
                      : Takes Texture2D as input parameter.
        - BilinearScale() : Cumulative function that combines all three methods in the class in one - for easy use in other scripts.
                        : Called from MeshInteractor script when Left Controller secondary button is pushed.

        // FBX METHODS ---------------------

        - PointScale() : Exports the scene as an FBX so it can be imported into other applications.

    // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    // ------------------------------------------------------------------------------------------
    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }
    // ------------------------------------------------------------------------------------------
    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }
    // ------------------------------------------------------------------------------------------
    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
            ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        }
        else
        {
            ratioX = ((float)tex.width) / newWidth;
            ratioY = ((float)tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        var slice = newHeight / cores;

        finishCount = 0;
        if (mutex == null)
        {
            mutex = new Mutex(false);
        }
        if (cores > 1)
        {
            int i = 0;
            ThreadData threadData;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                Thread thread = new Thread(ts);
                thread.Start(threadData);
            }
            threadData = new ThreadData(slice * i, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
            while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            ThreadData threadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
        }

        tex.Reinitialize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();

        texColors = null;
        newColors = null;
    }
    // ------------------------------------------------------------------------------------------
    public static void BilinearScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)Mathf.Floor(y * ratioY);
            var y1 = yFloor * w;
            var y2 = (yFloor + 1) * w;
            var yw = y * w2;

            for (var x = 0; x < w2; x++)
            {
                int xFloor = (int)Mathf.Floor(x * ratioX);
                var xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                       ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                       y * ratioY - yFloor);
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }
    // ------------------------------------------------------------------------------------------
    public static void PointScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            var thisY = (int)(ratioY * y) * w;
            var yw = y * w2;
            for (var x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }
    // ------------------------------------------------------------------------------------------
    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }
}
// --------------------------------------------------------------------------------------------------------------------------------------------------------------- END OF SCRIPT //