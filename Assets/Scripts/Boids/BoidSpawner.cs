using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    public int numBoids = 0;

    [SerializeField]
    public float boidDrag = 0.0f;

    [SerializeField]
    private Transform boid;

    [SerializeField]
    private BoidManager boidManager;

    [SerializeField]
    private List<Transform> boids = new List<Transform>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (numBoids < 0)
        {
            numBoids = 0;
        }

        for (int i = 0; i < boids.Count; i++)
        {
            Transform boid = boids[i];

            if (boid.position.sqrMagnitude > 100.0f)
            {
                Transform boidToRemove = boids[i];

                boids.RemoveAt(i);

                boidManager.RemoveBoid(boidToRemove);

                Destroy(boidToRemove.gameObject);
            }
        }

        if (numBoids == boids.Count) return;

        if (numBoids < boids.Count)
        {
            RemoveBoids(boids.Count - numBoids);
        }
        else if (numBoids > boids.Count)
        {
            SpawnMoreBoids(numBoids - boids.Count);
        }
    }



    private void SpawnMoreBoids(int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Vector3 randomLocationOnSphere = Random.onUnitSphere * 0.5f;
            Quaternion randomRotation = Random.rotation;
            Color randomColor = Random.ColorHSV();

            Transform newBoid = Instantiate(boid, randomLocationOnSphere, randomRotation);

            // newBoid.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", randomColor);
            // newBoid.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
            // newBoid.GetComponent<MeshRenderer>().material.color = randomColor;

            // newBoid.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * 5.0f;
            // newBoid.GetComponent<Rigidbody>().drag = boidDrag;

            boids.Add(newBoid);

            boidManager.AddBoid(newBoid, Random.onUnitSphere * 0.1f);
        }
    }



    private void RemoveBoids(int numToRemove)
    {
        for (int i = 0; i < numToRemove; i++)
        {
            int indexToRemvove = Random.Range(0, boids.Count);

            Transform boidToRemove = boids[indexToRemvove];

            boids.RemoveAt(indexToRemvove);

            boidManager.RemoveBoid(boidToRemove);

            Destroy(boidToRemove.gameObject);
        }
    }
}
