
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    public GrabInteractable ControllerGrabable;
    public HandGrabInteractable HandGrabable;

    void Start()
    {
        ControllerGrabable.WhenStateChanged += WhenStateChanged;
        HandGrabable.WhenStateChanged += WhenStateChanged;
    }



    private void OnDestroy()
    {
        ControllerGrabable.WhenStateChanged -= WhenStateChanged;
        HandGrabable.WhenStateChanged -= WhenStateChanged;
    }


    private void WhenStateChanged(InteractableStateChangeArgs args)
    {
        Debug.Log($"State changed to {args.NewState}");
    }
}
