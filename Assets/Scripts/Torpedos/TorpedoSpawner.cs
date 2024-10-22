
using UnityEngine;



[System.Obsolete("This class is for testing only, please use TorpedoManager instead")]
public class TorpedoSpawner : MonoBehaviour
{
    private TorpedoManager torpedoManager => TorpedoManager.Instance;

    [SerializeField]
    private float spawnPeriod;

    private float timeSinceLastSpawn = 0.0f;


    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnPeriod)
        {
            timeSinceLastSpawn = 0.0f;

            Transform newTorpedo = Instantiate(torpedoManager.TorpedoPrefab, transform.position + (0.2f * Random.onUnitSphere), Random.rotation);
            
            newTorpedo.parent = transform;
            
            torpedoManager.AddTorpedo(newTorpedo, 0.1f * Random.onUnitSphere, GameManager.Alliance.Enemy);
        }
    }
}
