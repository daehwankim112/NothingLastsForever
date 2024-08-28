
using UnityEngine;



public class BoidSpawner : Singleton<BoidSpawner>
{
    private GameManager gameManager => GameManager.Instance;

    private Settings settings => gameManager.Settings;

    private BoidManager boidManager => BoidManager.Instance;


    [SerializeField]
    private int numBoidsTarget;

    private int numBoids => boidManager.NumBoids;

    [SerializeField]
    private int numBoidsByEndOfSecond;

    [SerializeField]
    private float timeToEndOfSecond;

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
        timeToEndOfSecond -= Time.deltaTime;

        int numBoidsToSpawnThisFrame = Mathf.CeilToInt((Time.deltaTime / timeToEndOfSecond) * (numBoidsByEndOfSecond - numBoids));

        if (settings.BoidsRemovable && numBoidsToSpawnThisFrame < 0)
        {
            RemoveBoids(-numBoidsToSpawnThisFrame);
        }
        else if (numBoids < settings.BoidMax && numBoidsToSpawnThisFrame > 0)
        {
            SpawnMoreBoids(numBoidsToSpawnThisFrame);
        }

        if (timeToEndOfSecond <= 0.0f)
        {
            int boidDifference = numBoidsTarget - numBoids;
            int thisSecondSpawn = Mathf.CeilToInt(Mathf.Clamp(boidDifference, -settings.BoidMaxSpawnRate, settings.BoidMaxSpawnRate));

            numBoidsByEndOfSecond = numBoids + thisSecondSpawn;

            timeToEndOfSecond = 1.0f;
        }
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
        numBoidsTarget += Mathf.CeilToInt(settings.BoidMaxWaveContribution * args.DifficultyDelta / settings.BoidWeight);
    }
}
