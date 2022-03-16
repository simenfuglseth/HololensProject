
using System.Linq;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class ImageCapture : MonoBehaviour

{
    [SerializeField]
    private MixedRealityInputAction selectAction;
    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static ImageCapture Instance;

    PhotoCapture photoCaptureObject = null;
    
    /// <summary>
    /// File path of current analysed photo
    /// </summary>
    internal string filePath = string.Empty;

    private int captureCount = 0;
    /// <summary>
    /// Flagging if the capture loop is running
    /// </summary>
    internal bool captureIsActive;

    private void Awake()
    {
        Instance = this;

        //gameObject.AddComponent<CustomVisionAnalyser>();
    }
    // Use this for initialization
    void Start()
    {
        // Clean up the LocalState folder of this application from all photos stored
        DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            try
            {
                file.Delete();
            }
            catch (Exception)
            {
                Debug.LogFormat("Cannot delete file: ", file.Name);
            }
        }
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);
        // Begin the capture loop
        //Invoke("ExecuteImageCaptureAndAnalysis", 0);

    }

    //Button to take photo for testing
    void OnGUI()
    {
        
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
            //captureIsActive = true;

            // Set the cursor color to red
            //SceneOrganiser.Instance.cursor.GetComponent<Renderer>().material.color = Color.red;
            Invoke("ExecuteImageCaptureAndAnalysis", 0);
    }
            /// <summary>
            /// Begin process of image capturing and send to Azure Custom Vision Service.
            /// </summary>
    private void ExecuteImageCaptureAndAnalysis()
    {

        // Create a label in world space using the ResultsLabel class 
        // Invisible at this point but correctly positioned where the image was taken
        SceneOrganiser.Instance.PlaceAnalysisLabel();
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Begin capture process, set the image format
        PhotoCapture.CreateAsync(true, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            CameraParameters camParameters = new CameraParameters
            {
                hologramOpacity = 1.0f,
                cameraResolutionWidth = targetTexture.width,
                cameraResolutionHeight = targetTexture.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            // Capture the image from the camera and save it in the App internal folder
            captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                string filename = string.Format(@"CapturedImage{0}.jpg", captureCount);
                filePath = Path.Combine(Application.persistentDataPath, filename);
                captureCount++;
                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);

            });
        });
    }
    //Show the image catured
    /*
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // Create a gameobject that we can apply our texture shown in Quad scene
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);


        Debug.Log(Application.persistentDataPath);

    } */
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        try
        {
            // Call StopPhotoMode once the image has successfully captured
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);



        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception capturing photo to disk: {0}", e.Message);
        }
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        StartCoroutine(CustomVisionAnalyser.Instance.AnalyseLastImageCaptured(filePath));

    }

    /// <summary>
    /// Stops all capture pending actions
    /// </summary>
    internal void ResetImageCapture()
    {
        captureIsActive = false;

        // Set the cursor color to green
        SceneOrganiser.Instance.cursor.GetComponent<Renderer>().material.color = Color.green;

        // Stop the capture loop if active
        CancelInvoke();
    }

}




