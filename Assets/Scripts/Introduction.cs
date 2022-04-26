using UnityEngine;
using TMPro;
using System.Collections;

public class Introduction : MonoBehaviour
{
    public static Introduction Instance;

    private TextMeshPro TextMeshProo;
    private void Awake()
    {

        Instance = this;

        gameObject.AddComponent<ImageCapture>();

        TextMeshProo = gameObject.GetComponent<TextMeshPro>();
    }
    /// <summary>
    /// Hides the introduction from field of view for 30 seconds after image is captured
    /// </summary>
    private void hideIntro()
    {

        TextMeshProo.text = "";
        StartCoroutine(Example());
    }
    /// <summary>
    /// Places introduction text back after the time has passed
    /// </summary>
    /// <returns></returns>
    IEnumerator Example()
    {

        yield return new WaitForSecondsRealtime(30);

        TextMeshProo.text = "Say Capture Image to find object \nSay Remove Objects to restart scene";
    }
}
