
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;



public class Chest : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;

    private GrabInteractable ControllerGrabable;
    private HandGrabInteractable HandGrabable;

    public GameObject HandGrabObject;

    private bool opened = false;


    void Start()
    {
        HandGrabable = HandGrabObject.GetComponent<HandGrabInteractable>();
        ControllerGrabable = HandGrabObject.GetComponent<GrabInteractable>();

        ControllerGrabable.WhenStateChanged += OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged += OnGrabbableStateChanged;
    }



    void OnDestroy()
    {
        ControllerGrabable.WhenStateChanged -= OnGrabbableStateChanged;
        HandGrabable.WhenStateChanged -= OnGrabbableStateChanged;
    }


    [ContextMenu("Get Collected")]
    private void GetCollected()
    {
        gameManager.Death(gameObject, GameManager.Alliance.Player);
    }



    private void OnGrabbableStateChanged(InteractableStateChangeArgs args)
    {
        if (args.PreviousState == InteractableState.Hover || args.PreviousState == InteractableState.Select)
        {
            opened = true;
        }

        if (opened && (args.NewState == InteractableState.Normal || args.NewState == InteractableState.Disabled))
        {
            GetCollected();
        }
    }
}
