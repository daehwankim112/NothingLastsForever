using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingsThatDieSpawner : MonoBehaviour
{
    public GameObject ThingThatDiesPrefab;

    public float SpawnInterval = 1.0f;

    public int LeftToSpawn = 10;

    public float SpawnRadius = 100.0f;

    private float timeSinceLastSpawn = 0.0f;


    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= SpawnInterval && LeftToSpawn > 0)
        {
            timeSinceLastSpawn = 0.0f;
            LeftToSpawn--;

            GameObject newTtd = Instantiate(ThingThatDiesPrefab, transform.position + (SpawnRadius * Random.insideUnitSphere), Random.rotationUniform);

            newTtd.GetComponent<ThingThatDies>().DeathTimer = Random.Range(1, 20);
            newTtd.GetComponent<Inventory>().NumTorpedos = Random.Range(0, 1000);
        }
    }
}
