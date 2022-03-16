using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
public class SpeechManager : MonoBehaviour, IMixedRealitySpeechHandler
{
    void Start()
    {
        //Debugging "play sound" declarations deleted as not important for this example script.
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log("Image Capture");
        ImageCapture.Instance.Invoke("ExecuteImageCaptureAndAnalysis", 0);
    }



}
