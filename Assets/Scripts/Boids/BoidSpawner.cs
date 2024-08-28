
using UnityEngine;



public class BoidSpawner : Singleton<BoidSpawner>
{
    private GameManager gameManager => GameManager.Instance;

    private Settings settings => gameManager.Settings;

    private BoidManager boidManager => BoidManager.Instance;


    [SerializeField]
    private int numBoidsTarget;

    [SerializeField]
    private int numBoidsToSpawn;

    private int numBoids => boidManager.NumBoids;

    [SerializeField]
    private Transform boidPrefab;




    void Start()
    {
        gameManager.OnWave += OnWave;

        if (boidPrefab == null)
        {
            Debug.LogError("Boid prefab is not set in BoidSpawner. BoidSpawner will be disabled");
            enabled = false;
        }
    }



    void Update()
    {
        numBoidsToSpawn = numBoidsTarget - numBoids;

        if (numBoidsToSpawn <= 0) return;

        int spawnThisFrame = Mathf.Min(Mathf.FloorToInt(numBoidsToSpawn * Time.deltaTime), Mathf.Max(1, Mathf.FloorToInt(settings.BoidMaxSpawnRate * Time.deltaTime)));

        SpawnMoreBoids(spawnThisFrame);

        numBoidsToSpawn -= spawnThisFrame;
    }



    private void SpawnMoreBoids(int numToSpawn)
    {
        for (int i = 0; i < numToSpawn && numBoids < settings.BoidMax; i++)
        {
            Vector3 randomLocationOnSphere = Random.onUnitSphere * 0.25f;
            Quaternion randomRotation = Random.rotation;

            Transform newBoid = Instantiate(boidPrefab, transform.position + randomLocationOnSphere, randomRotation);
            boidManager.AddBoid(newBoid, Random.onUnitSphere * 0.1f);
        }
    }



    private void RemoveBoids(int numToRemove)
    {
        for (int i = 0; i < numToRemove; i++)
        {
            boidManager.RemoveBoid(true);
        }
    }



    private void OnWave(object sender, GameManager.OnWaveArgs args)
    {
        numBoidsTarget += Mathf.FloorToInt(settings.BoidMaxWaveContribution * args.DifficultyDelta / settings.BoidWeight);

        numBoidsTarget = Mathf.Clamp(numBoidsTarget, 0, settings.BoidMax);
    }
}
