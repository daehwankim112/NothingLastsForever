using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowHMD : MonoBehaviour
{
    public Transform targetTransform; // Reference to the target transform
    public Transform targetHMD; // Distance from the target transform
    public float height = 0.2f; // Offset from the target transform

    private void Start()
    {
        if (targetTransform == null)
        {
            Debug.LogWarning("Target Follow Transform is not assigned.");
            return;
        }
        
    }
    

    void Update()
    {
        // Get the forward direction relative to the target transform
        Vector3 forwardDirection = targetTransform.position + (targetTransform.right.normalized * height) + (targetTransform.up.normalized * height);

        // Set the position of the UI element to follow the gaze
        transform.position = forwardDirection; // Adjust the distance as needed

        // Calculate the direction from the UI element to the target
        Vector3 lookDirection = targetHMD.position - transform.position;

        // Optionally, make the UI element face away from the target
        transform.rotation = Quaternion.LookRotation(-lookDirection);
    }
}
