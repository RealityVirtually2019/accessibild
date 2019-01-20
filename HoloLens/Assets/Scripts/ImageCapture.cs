using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;

public class ImageCapture : MonoBehaviour
{

    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static ImageCapture Instance;

    /// <summary>
    /// Keep counts of the taps for image renaming
    /// </summary>
    private int captureCount = 0;

    /// <summary>
    /// Photo Capture object
    /// </summary>
    private PhotoCapture photoCaptureObject = null;

    /// <summary>
    /// Loop timer
    /// </summary>
    private float secondsBetweenCaptures = 3f;

    /// <summary>
    /// Flagging if the capture loop is running
    /// </summary>
    internal bool captureIsActive;

    /// <summary>
    /// File path of current analysed photo
    /// </summary>
    internal string filePath = string.Empty;

    private bool cameraResolutionSet = false;

    private Resolution cameraResolution;

    public bool IsRecording { get; private set; }

    /// <summary>
    /// Called on initialization
    /// </summary>
    private void Awake()
    {
        Instance = this;

    }

    /// <summary>
    /// Runs at initialization right after Awake method
    /// </summary>
    void Start()
    {
        // Clean up the LocalState folder of this application from all photos stored
        DirectoryInfo info = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "bathroom"));
        if (!info.Exists)
        {
            info.Create();
        }

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
    }

    public void StartRecording()
    {
        IsRecording = true;
        // Begin the capture loop
        InvokeRepeating("ExecuteImageCaptureAndAnalysis", 0, secondsBetweenCaptures);
    }

    private void OnDestroy()
    {
        CancelInvoke();

        if (photoCaptureObject != null)
        {
            // Dispose from the object in memory 
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }
    }

    /// <summary>
    /// Begin process of Image Capturing and send To Azure Custom Vision Service.
    /// </summary>
    private void ExecuteImageCaptureAndAnalysis()
    {
        if (!cameraResolutionSet)
        {
            cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            cameraResolutionSet = true;
        }
        {
            // Set the camera resolution to be the highest possible

            //Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            // Begin capture process, set the image format
            PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
            {
                photoCaptureObject = captureObject;

                CameraParameters camParameters = new CameraParameters
                {
                    hologramOpacity = 0.0f,
                    cameraResolutionWidth = cameraResolution.width,
                    cameraResolutionHeight = cameraResolution.height,
                    pixelFormat = CapturePixelFormat.BGRA32
                };

                // Capture the image from the camera and save it in the App internal folder
                captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
                    {
                        string filename = string.Format(@"CapturedImage{0}.jpg", captureCount);
                        filePath = Path.Combine(Path.Combine(Application.persistentDataPath, "bathroom"), filename);
                        captureCount++;
                        photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
                    });
            });

        }
    }

    void AnalyzeImage()
    {

        if (!captureIsActive)
        {
            captureIsActive = true;
        }
        else
        {
            // The user tapped while the app was analyzing 
            // therefore stop the analysis process
            ResetImageCapture();
        }
    }

    /// <summary>
    /// Register the full execution of the Photo Capture. 
    /// </summary>
    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        // Call StopPhotoMode once the image has successfully captured
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }


    /// <summary>
    /// The camera photo mode has stopped after the capture.
    /// Begin the Image Analysis process.
    /// </summary>
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.LogFormat("Stopped Photo Mode");

        // Dispose from the object in memory and request the image analysis 
        photoCaptureObject.Dispose();
        photoCaptureObject = null;

        // Call the image analysis
        StartCoroutine(CustomVisionAnalyser.Instance.AnalyseLastImageCaptured(filePath));
    }

    /// <summary>
    /// Stops all capture pending actions
    /// </summary>
    internal void ResetImageCapture()
    {
        IsRecording = false;
        captureIsActive = false;

        // Stop the capture loop if active
        CancelInvoke();
    }

}
