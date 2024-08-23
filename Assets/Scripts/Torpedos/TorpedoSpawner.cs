
using UnityEngine;

public class TorpedoSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform torpedo;

    [SerializeField]
    private TorpedoManager torpedoManager;

    [SerializeField]
    private float spawnPeriod;

    private float timeSinceLastSpawn = 0.0f;


    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnPeriod)
        {
            timeSinceLastSpawn = 0.0f;

            Transform newTorpedo = Instantiate(torpedo, transform.position + (0.2f * Random.onUnitSphere), transform.rotation);

            torpedoManager.AddTorpedo(newTorpedo, 0.1f * Random.onUnitSphere, TorpedoManager.TorpedoAlliance.Enemy);
        }
    }
}
