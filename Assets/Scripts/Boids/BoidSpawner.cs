
using UnityEngine;



public class BoidSpawner : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;

    private Settings settings => gameManager.Settings;


    [SerializeField]
    private int numBoidsTarget;

    private int numBoids => boidManager.NumBoids;

    [SerializeField]
    private int numBoidsByEndOfSecond;

    [SerializeField]
    private float timeToEndOfSecond;

    [SerializeField]
    private Transform boid;

    [SerializeField]
    private BoidManager boidManager;



    void Start()
    {
        gameManager.OnWave += OnWave;
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
        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 randomLocationOnSphere = Random.onUnitSphere * 0.25f;
            Quaternion randomRotation = Random.rotation;
            Color randomColor = Random.ColorHSV();

            Transform newBoid = Instantiate(boid, transform.position + randomLocationOnSphere, randomRotation);

            // newBoid.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", randomColor);
             newBoid.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
            newBoid.GetComponent<MeshRenderer>().material.color = randomColor;

            // newBoid.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * 5.0f;
            // newBoid.GetComponent<Rigidbody>().drag = boidDrag;

            boidManager.AddBoid(newBoid, Random.onUnitSphere * 0.1f);
        }

        Debug.Log($"Boid Difficulty: {settings.BoidWeight * numBoids}");
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
