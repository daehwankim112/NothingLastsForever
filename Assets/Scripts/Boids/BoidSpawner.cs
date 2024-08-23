using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    public int numBoids = 0;

    [SerializeField]
    private Transform boid;

    [SerializeField]
    private BoidManager boidManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (numBoids == boidManager.NumBoids) return;

        if (numBoids < boidManager.NumBoids)
        {
            RemoveBoids(boidManager.NumBoids - numBoids);
        }
        else if (numBoids > boidManager.NumBoids)
        {
            SpawnMoreBoids(numBoids - boidManager.NumBoids);
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
             newBoid.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
            newBoid.GetComponent<MeshRenderer>().material.color = randomColor;

            // newBoid.GetComponent<Rigidbody>().velocity = Random.onUnitSphere * 5.0f;
            // newBoid.GetComponent<Rigidbody>().drag = boidDrag;

            boidManager.AddBoid(newBoid, Random.onUnitSphere * 0.1f);
        }
    }



    private void RemoveBoids(int numToRemove)
    {
        for (int i = 0; i < numToRemove; i++)
        {
            boidManager.RemoveBoid(null, true);
        }
    }
}
