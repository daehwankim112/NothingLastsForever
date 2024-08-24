
using UnityEngine;

public class TorpedoExposionRecieverTest : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;



    private void Start()
    {
        gameManager.OnExplosion += OnTorpedoExploded;
    }



    private void OnTorpedoExploded(object sender, GameManager.OnExplosionArgs explosionArgs)//Vector3 position, float power, GameManager.Alliance alliance)
    {
        float distance = Vector3.Distance(explosionArgs.Position, transform.position);

        float explosionPower = explosionArgs.Power / (distance * distance);

        Debug.Log($"Torpedo of type {(explosionArgs.ExplosionAlliance == GameManager.Alliance.Player ? "Player" : "Enemy")} exploded at {explosionArgs.Position} with power {explosionArgs.Power}, felt as {explosionPower:F2} {distance:F2} meters away");
    }


    private void OnDestroy()
    {
        var torpedoManager = FindObjectOfType<TorpedoManager>();
        if (torpedoManager == null) return;

        gameManager.OnExplosion -= OnTorpedoExploded;
    }
}
