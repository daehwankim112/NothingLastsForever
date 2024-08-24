
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;

public class Collectable : MonoBehaviour
{
    private GrabInteractable ControllerGrabable;
    private HandGrabInteractable HandGrabable;

    private Inventory inventory;


    void Start()
    {
        inventory = GetComponent<Inventory>();

        HandGrabable = gameObject.GetComponent<HandGrabInteractable>();
        ControllerGrabable = gameObject.GetComponent<GrabInteractable>();

        ControllerGrabable.WhenStateChanged += OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged += OnGrabbableStateChanged;
    }



    private void OnDestroy()
    {
        ControllerGrabable.WhenStateChanged -= OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged -= OnGrabbableStateChanged;
    }



    private void OnGrabbableStateChanged(InteractableStateChangeArgs args)
    {
        Debug.Log($"State changed from {args.PreviousState} to {args.NewState}");

        if (args.PreviousState == InteractableState.Select)
        {
            Debug.Log("Object was dropped");

            GetComponent<Rigidbody>().AddTorque(100.0f * Vector3.forward);
        }
    }
}
