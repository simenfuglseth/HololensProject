using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GazeCursor : MonoBehaviour
{


    /// <summary>
    /// The cursor (this object) mesh renderer
    /// </summary>
    private MeshRenderer meshRenderer;
    /// <summary>
    /// Runs at initialization right after the Awake method
    /// </summary>
    void Start()
    {
        // Grab the mesh renderer that is on the same object as this script.
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        // Set the cursor reference
        SceneOrganiser.Instance.cursor = gameObject;
        gameObject.GetComponent<Renderer>().material.color = Color.white;

        // If you wish to change the size of the cursor you can do so here
        meshRenderer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);


    }
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Do a raycast into the world based on the user's head position and orientation.
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        RaycastHit gazeHitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out gazeHitInfo, 30.0f, Physics.DefaultRaycastLayers))
        {

        }
        else
        {

        }

    }
}
