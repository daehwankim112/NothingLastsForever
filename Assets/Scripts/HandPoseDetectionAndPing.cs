using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseDetectionAndPing : MonoBehaviour
{
    [HideInInspector] public bool pingGestureActivated = false;
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform leftHandPinchArea;
    [SerializeField] private Transform rightHandPinchArea;
    private bool holdingLeftHand = false;
    private bool holdingRightHand = false;

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

    public void LeftFire()
    {
        Debug.Log("Left Fire!");
        Instantiate(projectile, leftHandPinchArea.position, leftHandPinchArea.rotation);
    }

    public void RightFire()
    {
        Debug.Log("Right Fire!");
        Instantiate(projectile, rightHandPinchArea.position, rightHandPinchArea.rotation);
    }
}
