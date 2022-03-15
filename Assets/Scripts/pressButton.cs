using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressButton : MonoBehaviour, IMixedRealityPointerHandler
{
    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        ImageCapture.Instance.Invoke("ExecuteImageCaptureAndAnalysis", 0);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        
    }

}
