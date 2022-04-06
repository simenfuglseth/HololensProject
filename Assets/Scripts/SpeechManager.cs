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
        Debug.Log("OnSpeechKeywordRecognized");

        switch (eventData.Command.Keyword)

        {
            case "Image Capture":
                ImageCapture.Instance.Invoke("ExecuteImageCaptureAndAnalysis", 0);
                Debug.Log("Image Capture");

                break;

            case "Remove Objects":
                SceneOrganiser.Instance.Invoke("reload", 0);
                Debug.Log("Remove Objects");

                break;
        }
    }
}
