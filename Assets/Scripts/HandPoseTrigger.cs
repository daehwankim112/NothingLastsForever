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
    }



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
