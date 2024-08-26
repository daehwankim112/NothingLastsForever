
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;

public class Collectable : MonoBehaviour
{
    private CollectablesManager collectablesManager => CollectablesManager.Instance;

    private GrabInteractable ControllerGrabable;
    private HandGrabInteractable HandGrabable;

    private Inventory inventory;

    public GameObject HandGrabObject;


    void Start()
    {
        inventory = GetComponent<Inventory>();

        HandGrabable = HandGrabObject.GetComponent<HandGrabInteractable>();
        ControllerGrabable = HandGrabObject.GetComponent<GrabInteractable>();

        ControllerGrabable.WhenStateChanged += OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged += OnGrabbableStateChanged;
    }



    private void OnDestroy()
    {
        ControllerGrabable.WhenStateChanged -= OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged -= OnGrabbableStateChanged;
    }



    private void GetCollected()
    {
        collectablesManager.CollectCollectable(gameObject);
    }



    private void OnGrabbableStateChanged(InteractableStateChangeArgs args)
    {
        if (args.PreviousState == InteractableState.Select)
        {
            GetCollected();
        }
    }
}
