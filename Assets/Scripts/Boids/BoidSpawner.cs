using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform boid;

    [SerializeField]
    public int numBoids = 0;

    [SerializeField]
    private List<Transform> boids = new List<Transform>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (numBoids == boids.Count) return;

        if (numBoids < boids.Count)
        {
            SpawnMoreBoids(boids.Count - numBoids);
        }
        else if (numBoids > boids.Count)
        {
            RemoveBoids(numBoids - boids.Count);
        }
    }



    private void SpawnMoreBoids(int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 randomLocationOnSphere = Random.onUnitSphere * 2f;
            Quaternion randomRotation = Random.rotation;
            Color randomColor = Random.ColorHSV();

            Transform newBoid = Instantiate(boid, randomLocationOnSphere, randomRotation);

            newBoid.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", randomColor);
            newBoid.GetComponent<MeshRenderer>().material.color = randomColor;

            boids.Add(newBoid);
        }
    }



    private void RemoveBoids(int numToRemove)
    {
        for (int i = 0; i < numToRemove; i++)
        {
            int indexToRemvove = Random.Range(0, boids.Count);

            Transform boidToRemove = boids[indexToRemvove];

            boids.RemoveAt(indexToRemvove);

            Destroy(boidToRemove);
        }
    }
}
