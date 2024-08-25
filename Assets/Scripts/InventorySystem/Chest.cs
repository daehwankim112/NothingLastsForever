
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using UnityEngine;

public class Chest : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;

    private GrabInteractable ControllerGrabable;
    private HandGrabInteractable HandGrabable;

    public GameObject HandGrabObject;


    void Start()
    {
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



    private void OnGrabbableStateChanged(InteractableStateChangeArgs args)
    {
        if (args.PreviousState == InteractableState.Hover)
        {
            gameManager.Death(gameObject, GameManager.Alliance.Player);

            Destroy(gameObject);
        }
    }
}
