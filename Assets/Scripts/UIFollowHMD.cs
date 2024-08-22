using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowHMD : MonoBehaviour
{

    public Transform targetTransform; // Reference to the target transform
    public float distance = 2.0f; // Distance from the target transform
    public Vector3 UIOffsetFromAnchor = Vector3.zero; // Offset from the target transform

    void Update()
    {
        if (targetTransform == null)
        {
            Debug.LogWarning("Target Follow Transform is not assigned.");
            return;
        }

        // Get the gaze direction from the target transform
        Vector3 gazeDirection = targetTransform.position + UIOffsetFromAnchor;

        // Set the position of the UI element to follow the gaze
        transform.position = targetTransform.position + gazeDirection * distance; // Adjust the distance as needed

        // Optionally, make the UI element face the target
        //transform.LookAt(transform.position - (targetTransform.position - transform.position));
    }
}
