using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseTrigger : MonoBehaviour
{
    [HideInInspector] public bool pingGestureActivated = false;
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform leftHandPinchArea;
    [SerializeField] private Transform rightHandPinchArea;
    private bool holdingLeftHand = false;
    private bool holdingRightHand = false;

    public void LeftHandPing()
    {
        Debug.Log("Left Ping!");
        holdingLeftHand = true;
    }

    public void LeftHandUnping()
    {
        Debug.Log("Left Unping!");
        holdingLeftHand = false;
        pingGestureActivated = true;
    }

    public void RightHandPing()
    {
        Debug.Log("Right Ping!");
        holdingRightHand = true;
    }

    public void RightHandUnping()
    {
        Debug.Log("Right Unping!");
        holdingRightHand = false;
        pingGestureActivated = true;
    }

    public void LeftHandFire()
    {
        Debug.Log("Left Fire!");
        Instantiate(projectile, leftHandPinchArea.position, leftHandPinchArea.rotation);
    }

    public void RightHandFire()
    {
        Debug.Log("Right Fire!");
        Instantiate(projectile, rightHandPinchArea.position, rightHandPinchArea.rotation);
    }
}
