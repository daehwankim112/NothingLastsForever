
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
        if (HandGrabObject == null)
        {
            Debug.LogError("HandGrabObject is not set in Chest prefab");
            HandGrabObject = GetComponentInParent<HandGrabInteractable>().gameObject;
        }

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



    [ContextMenu("Get Collected")]
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
