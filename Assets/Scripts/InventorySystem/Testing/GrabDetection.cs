using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    public HandGrabInteractable handGrabInteractable;

    void Start()
    {
        handGrabInteractable.WhenStateChanged += WhenStateChanged;
    }

    private void WhenStateChanged(InteractableStateChangeArgs args)
    {
        Debug.Log($"State changed to {args.NewState}");
    }
}
