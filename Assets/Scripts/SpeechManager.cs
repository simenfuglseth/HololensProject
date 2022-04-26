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
    /// <summary>
    /// Add custom made word commands to be recongized and event
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        Debug.Log("OnSpeechKeywordRecognized");

        switch (eventData.Command.Keyword)

        {
            case "Capture Image":
                ImageCapture.Instance.Invoke("ExecuteImageCaptureAndAnalysis", 0);
                Debug.Log("Capture Image");

                break;

            case "Remove Objects":
                SceneOrganiser.Instance.Invoke("reload", 0);
                Debug.Log("Remove Objects");

                break;
        }
    }
}
