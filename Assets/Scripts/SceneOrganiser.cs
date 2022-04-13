using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOrganiser : MonoBehaviour
{
    /// <summary>
    /// Sends raycast to update
    /// </summary>
    internal bool sendRaycast;
    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static SceneOrganiser Instance;

    /// <summary>
    /// The cursor object attached to the Main Camera
    /// </summary>
    internal GameObject cursor;

    /// <summary>
    /// The label used to display the analysis on the objects in the real world
    /// </summary>
    internal GameObject label;

    /// <summary>
    /// Reference to the last Label positioned
    /// </summary>
    internal Transform lastLabelPlaced;

    /// <summary>
    /// Reference to the last Label positioned
    /// </summary>
    internal TextMesh lastLabelPlacedText;

    /// <summary>
    /// Current threshold accepted for displaying the label
    /// Reduce this value to display the recognition more often
    /// </summary>
    internal float probabilityThreshold = 0.5f;

    /// <summary>
    /// The quad object hosting the imposed image captured
    /// </summary>
    private GameObject quad;

    /// <summary>
    /// Renderer of the quad object
    /// </summary>
    internal Renderer quadRenderer;


    /// <summary>
    /// Called on initialization
    /// </summary>

    private void Awake()
    {
        
        // Use this class instance as singleton
        Instance = this;

        // Add the ImageCapture class to this Gameobject
        gameObject.AddComponent<ImageCapture>();

        // Add the CustomVisionAnalyser class to this Gameobject
        gameObject.AddComponent<CustomVisionAnalyser>();

        // Add the CustomVisionObjects class to this Gameobject
        gameObject.AddComponent<CustomVisionObjects>();

        // Load the label prefab as reference
        label = CreateLabel();

    }
    /// <summary>
    /// Create the analysis label object
    /// </summary>
    private GameObject CreateLabel()
    {
   
        GameObject newLabel = new GameObject();

        // Creating the text of the label
        TextMesh t = newLabel.AddComponent<TextMesh>();
        t.anchor = TextAnchor.MiddleCenter;
        t.alignment = TextAlignment.Center;
        t.fontSize = 100;
        t.text = "";

        return newLabel;
    }

    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void PlaceAnalysisLabel()
    {
        //Destroy previous quad
        //Destroy(quad);

        lastLabelPlaced = Instantiate(label.transform, cursor.transform.position, transform.rotation);

        lastLabelPlaced.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        // Create a GameObject to which the texture can be applied
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = quad.GetComponent<Renderer>();

        // Makes quad invincible
        quad.GetComponent<Renderer>().enabled = false;

        // Set the position and scale of the quad depending on user position
        quad.transform.parent = transform;
        quad.transform.rotation = transform.rotation;

        // The quad is positioned slightly forward in front of the user
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        // The quad scale as been set with the following value following experimentation,  
        // to allow the image on the quad to be as precisely imposed to the real world as possible
        quad.transform.localScale = new Vector3(3f, 1.65f, 1f);
        quad.transform.parent = null;

    }
    /// <summary>
    /// Set the Tags as Text of the last label created. 
    /// </summary>
    public void FinaliseLabel(AnalysisRootObject analysisObject)
    {
        if (analysisObject.predictions != null)
        {
            lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();
            // Sort the predictions to locate the highest one
            List<Prediction> sortedPredictions = new List<Prediction>();
            sortedPredictions = analysisObject.predictions.OrderBy(p => p.probability).ToList();
            Prediction bestPrediction = new Prediction();
            bestPrediction = sortedPredictions[sortedPredictions.Count - 1];
            Debug.Log(bestPrediction.probability);
            if (bestPrediction.probability > probabilityThreshold)
            {
                quadRenderer = quad.GetComponent<Renderer>();
                Bounds quadBounds = quadRenderer.bounds;

                // Position the label as close as possible to the Bounding Box of the prediction 
                // At this point it will not consider depth
  
                lastLabelPlaced.transform.parent = quad.transform;
                lastLabelPlaced.transform.localPosition = CalculateBoundingBoxPosition(quadBounds, bestPrediction.boundingBox);                   
                // Set the tag text
                lastLabelPlacedText.text = bestPrediction.tagName;

                sendRaycast = true;

            }
        }
        // Reset the color of the cursor
        cursor.GetComponent<Renderer>().material.color = Color.green;
        //Enables quad for debugging
        //quad.GetComponent<Renderer>().enabled = true;
        // Stop the analysis process
        ImageCapture.Instance.ResetImageCapture();
    }

    /// <summary>
    /// This method hosts a series of calculations to determine the position 
    /// of the Bounding Box on the quad created in the real world
    /// by using the Bounding Box received back alongside the Best Prediction
    /// Calculates the center of boundingbox from the respone which is a 0, 1 cordinate system
    /// to -0.5, 0.5 where the center is 0, 0 cordinate system
    /// </summary>
    public Vector3 CalculateBoundingBoxPosition(Bounds b, BoundingBox boundingBox)
    {
        Debug.Log($"BB: left {boundingBox.left}, top {boundingBox.top}, width {boundingBox.width}, height {boundingBox.height}");

        double centerFromLeft = boundingBox.left + (boundingBox.width / 2);
        double centerFromTop = boundingBox.top + (boundingBox.height / 2);
        Debug.Log($"BB CenterFromLeft {centerFromLeft}, CenterFromTop {centerFromTop}");

        if ((boundingBox.top > 0.5) & (boundingBox.left > 0.5))
        {
            double normalisedPos_X = (centerFromLeft - 0.5);
            double normalisedPos_Y = (0.5 - centerFromTop);
            Debug.Log($"Down right X {normalisedPos_X}, Y {normalisedPos_Y}");
            return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
        }
        else if ((boundingBox.top > 0.5) & (boundingBox.left < 0.5))
        {
            double normalisedPos_X = (centerFromLeft - 0.5);
            double normalisedPos_Y = (0.5 - centerFromTop);
            Debug.Log($"Down left X {normalisedPos_X}, Y {normalisedPos_Y}");
            return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
        }
        else if ((boundingBox.top < 0.5) & (boundingBox.left < 0.5))
        {
            double normalisedPos_X = (centerFromLeft - 0.5);
            double normalisedPos_Y = (0.5 - centerFromTop);
            Debug.Log($"Top left X {normalisedPos_X}, Y {normalisedPos_Y}");
            return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
        }
        else if ((boundingBox.top < 0.5) & (boundingBox.left > 0.5))
        {
            double normalisedPos_X = (centerFromLeft - 0.5);
            double normalisedPos_Y = (0.5 - centerFromTop);
            Debug.Log($"Top right X {normalisedPos_X}, Y {normalisedPos_Y}");
            return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }
    /// <summary>
    /// Reloads the scene
    /// </summary>
        private void reload()
    {
        SceneManager.LoadScene("MRKTscene");
    }
    void FixedUpdate()
    {
        RaycastHit objHitInfo;
        if (sendRaycast == true)
        {
            Debug.Log("Repositioning Label");
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 dir = lastLabelPlaced.transform.position - Camera.main.transform.position;
            //Debug.Log(dir);
            LineRenderer laserline;
            laserline = GetComponent<LineRenderer>();
            laserline.SetPosition(0, headPosition);
            if (Physics.Raycast(headPosition, dir, out objHitInfo, 30.0f, Physics.DefaultRaycastLayers))
            {
                lastLabelPlaced.position = objHitInfo.point;
                //Debug.Log(objHitInfo.point);
                laserline.SetPosition(1, objHitInfo.point);
                Debug.DrawRay(headPosition, dir, Color.blue, 1000000);
            }        
            sendRaycast = false;
        }
    }
}
