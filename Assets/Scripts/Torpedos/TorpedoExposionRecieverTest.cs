
using UnityEngine;

public class TorpedoExposionRecieverTest : MonoBehaviour
{
    private void Start()
    {
        var torpedoManager = FindObjectOfType<TorpedoManager>();
        if (torpedoManager == null) return;

        torpedoManager.OnTorpedoExploded += OnTorpedoExploded;
    }

    private void OnTorpedoExploded(Vector3 position, float power, TorpedoManager.TorpedoAlliance alliance)
    {
        float distance = Vector3.Distance(position, transform.position);

        float explosionPower = power / (distance * distance);

        Debug.Log($"Torpedo of type {(alliance == TorpedoManager.TorpedoAlliance.Player ? "Player" : "Enemy")} exploded at {position} with power {power}, felt as {explosionPower:F2} {distance:F2} meters away");
    }


    private void OnDestroy()
    {
        var torpedoManager = FindObjectOfType<TorpedoManager>();
        if (torpedoManager == null) return;

        torpedoManager.OnTorpedoExploded -= OnTorpedoExploded;
    }
}