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

    private void hideIntro()
    {

        TextMeshProo.text = "";
        StartCoroutine(Example());
    }
    IEnumerator Example()
    {

        yield return new WaitForSecondsRealtime(30);

        TextMeshProo.text = "Say Capture Image to find object \nSay Remove Objects to restart scene";
    }
}
