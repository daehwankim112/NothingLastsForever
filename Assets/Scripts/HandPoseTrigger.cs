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

    private TorpedoManager torpedoManager => TorpedoManager.Instance;
    private Inventory inventory;


    void Start()
    {
        inventory = GetComponent<Inventory>();
        pingGestureActivated = false;
        holdingLeftHand = false;
        holdingRightHand = false;
    }



    public void LeftHandHoldPing()
    {
        Debug.Log("Left Hold Ping!");
        holdingLeftHand = true;
    }

    public void LeftHandReleasePing()
    {
        if (holdingLeftHand)
        {
            Debug.Log("Left Release Ping!");
            holdingLeftHand = false;
            pingGestureActivated = true;
        }
    }

    public void RightHandHoldPing()
    {
        Debug.Log("Right Hold Ping!");
        holdingRightHand = true;
    }

    public void RightHandReleasePing()
    {
        if (holdingRightHand)
        {
            Debug.Log("Right Release Ping!");
            holdingRightHand = false;
            pingGestureActivated = true;
        }
    }

    public void LeftHandFire()
    {
        Debug.Log("Left Fire!");

        torpedoManager.ExplodeAllTorpedos(GameManager.Alliance.Player);
    }

    public void RightHandFire()
    {
        Debug.Log("Right Fire!");

        SpawnNewProjectile(rightHandPinchArea);
    }



    private void SpawnNewProjectile(Transform hand)
    {
        if (inventory.GetTorpedoes() <= 0)
        {
            Debug.Log("No torpedos left!");
            return;
        }

        Transform newProjectile = Instantiate(projectile, hand.position, hand.rotation);

        torpedoManager.AddTorpedo(newProjectile, hand.rotation * Vector3.forward, GameManager.Alliance.Player);
    }
}
