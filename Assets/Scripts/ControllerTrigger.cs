using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTrigger : MonoBehaviour
{
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            LeftControllerFire();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            RightControllerFire();
        }
    }

    public void LeftControllerFire()
    {
        Debug.Log("Left Fire!");
        Instantiate(projectile, leftControllerTransform.position, leftControllerTransform.rotation);
    }

    public void RightControllerFire()
    {
        Debug.Log("Right Fire!");
        Instantiate(projectile, rightControllerTransform.position, rightControllerTransform.rotation);
    }
}
