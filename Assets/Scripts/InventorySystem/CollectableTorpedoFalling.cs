using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollectableTorpedoFalling : MonoBehaviour
{
    private Rigidbody rb;
    private TorpedoManager torpedoManager = TorpedoManager.Instance;
    private MRUK mruk => MRUK.Instance;
    private MRUKRoom mrukRoom;
    private MRUKAnchor floorAnchor;

    private float timeSinceSpawned = 0;
    private bool spawned = false;
    private int numOfRays = 10;
    private float radius = 0.15f;

    private void Start()
    {
        mrukRoom = mruk.GetCurrentRoom();
        floorAnchor = mrukRoom.GetFloorAnchor();
        rb = GetComponent<Rigidbody>();
        rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
            ForceMode.Impulse);
        torpedoManager.AddFallingTorpedo(transform, Vector3.zero, GameManager.Alliance.Enemy);
    }

    private void Update()
    {
        if (!spawned)
        {
            timeSinceSpawned += Time.deltaTime;
            if (timeSinceSpawned >= 1f)
            {
                spawned = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude < 0.1f)
        {
            rb.AddForce(Physics.gravity / 20f, ForceMode.Acceleration);
        }
        if (collisionDetected())
        {
            torpedoManager.ExplodeFallingTorpedo(transform);
        }
    }


    private bool collisionDetected()
    {
        if (
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitDown, radius, 7) ||
            Physics.Raycast(transform.position, Vector3.left, out RaycastHit hitLeft, radius, 7) ||
            Physics.Raycast(transform.position, Vector3.right, out RaycastHit hitRight, radius, 7) ||
            Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitUp, radius, 7)
        )
        {
            return true;
        }
        return false;
    }
}
