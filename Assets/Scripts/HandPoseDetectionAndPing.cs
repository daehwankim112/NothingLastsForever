using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseDetectionAndPing : MonoBehaviour
{
    [HideInInspector] public bool pingGestureActivated = false;
    bool holdingLeftHand = false;
    bool holdingRightHand = false;

    public void LeftPing()
    {
        Debug.Log("Left Ping!");
        holdingLeftHand = true;
    }

    public void LeftUnping()
    {
        Debug.Log("Left Unping!");
        holdingLeftHand = false;
        pingGestureActivated = true;
    }

    public void RightPing()
    {
        Debug.Log("Right Ping!");
        holdingRightHand = true;
    }

    public void RightUnping()
    {
        Debug.Log("Right Unping!");
        holdingRightHand = false;
        pingGestureActivated = true;
    }
}
