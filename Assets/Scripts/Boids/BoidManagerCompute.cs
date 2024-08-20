using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BoidManager;

public class BoidManagerCompute : MonoBehaviour
{
    [SerializeField]
    // Scale the force of the boid manager
    public float BoidFactor = 1.0f;


    [SerializeField]
    // How aggressivly the boids will avoid each other
    public float SeparationFactor = 1.0f;

    [SerializeField]
    // The distance boids will stay away from each other
    public float SeparationDistance = 1.0f;

    [SerializeField]
    // Will the boids avoid boids they can't see
    public bool SeparationUsesFov = false;


    [SerializeField]
    // How aggressivly boids will match the velocity of boids around them
    public float AlignmentFactor = 1.0f;

    [SerializeField]
    // The max distance for alignment
    public float AlignmentDistance = 1.0f;

    [SerializeField]
    // Will the boids align with boids they can't see
    public bool AlignmentUsesFov = false;


    [SerializeField]
    // How aggressivly boids will stick together
    public float CohesionFactor = 1.0f;

    [SerializeField]
    // The max distance for sticking together
    public float CohesionDistance = 1.0f;

    [SerializeField]
    // Will the boids go to boids they can't see
    public bool CohesionUsesFov = false;


    [SerializeField]
    // How aggressivly boids will move towards the target
    public float TargetFactor = 1.0f;

    [SerializeField]
    // The max distance for targeting
    public float MaxTargetDistance = 1.0f;

    [SerializeField]
    // Will the boids go to targets they can't see
    public bool TargetUsesFov = false;

    [SerializeField]
    // Boid target mode
    public BoidRuleMode targetMode = BoidRuleMode.Average;

    [SerializeField]
    // The targets boids will move towards
    public List<Transform> targets = new();


    [SerializeField]
    // How aggressivly boids will avoid avoids
    public float AvoidFactor = 1.0f;

    [SerializeField]
    // The max distance for avoiding
    public float MaxAvoidDistance = 1.0f;

    [SerializeField]
    // Will the boids avoid avoids they can't see
    public bool AvoidUsesFov = false;

    [SerializeField]
    // Boid avoid mode
    public BoidRuleMode avoidMode = BoidRuleMode.Average;

    [SerializeField]
    // The things boids will move away from
    public List<Transform> avoids = new();


    [SerializeField]
    // Limit to how far boids can go
    public float boundingDistance = 100.0f;

    [SerializeField]
    // When the boids start turning back to not go too far
    public float boundingEffectDistance = 90.0f;


    [SerializeField]
    // The maximum force the boid manager will apply
    public float maxForce = 10.0f;

    [SerializeField]
    // The minimum force the boid manager will apply
    public float minForce = 1.0f;


    [SerializeField]
    // The range of angles boids will see other boids
    public float boidFov = 45.0f;


    [SerializeField]
    // The list of boids this manager manages
    public readonly List<Transform> boids = new();

    private readonly List<Rigidbody> boidRigidbodies = new();


    private List<int> frameBoids;
    private List<Vector3> frameBoidPositions;
    private List<Vector3> frameBoidVelocities;
    private List<Vector3> frameTargetPositions;
    private List<Vector3> frameAvoidPositions;


    [SerializeField]
    public ComputeShader BoidComputeShader;

    ComputeBuffer boidPositionsBuffer, boidVelocitiesBuffer, targetPositionsBuffer, avoidPositionsBuffer, boidForcesBuffer;



    /// <summary>
    /// Add a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to add.
    /// </param>
    public void AddBoid(Transform boid)
    {
        if (boid == null) return;

        if (!boid.TryGetComponent<Rigidbody>(out Rigidbody boidRigidbody)) return;

        boids.Add(boid);
        boidRigidbodies.Add(boidRigidbody);
    }



    /// <summary>
    /// Remove a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to remove.
    /// </param>
    /// <returns>
    /// True if the boid was successfully removed.
    /// False otherwise.
    /// </returns>
    public bool RemoveBoid(Transform boid)
    {
        Rigidbody boidRigidbody = boid.GetComponent<Rigidbody>();

        return boids.Remove(boid)
            && boidRigidbodies.Remove(boidRigidbody);
    }



    void FixedUpdate()
    {
        UpdateBoids();
    }



    private void UpdateBoids()
    {
        // Fill frame lists
        int numBoids = boids.Count;
        int numTargets = targets.Count;
        int numAvoids = avoids.Count;

        if (numBoids * numTargets * numAvoids == 0) return;

        frameBoids = Enumerable.Range(0, numBoids).ToList();
        frameBoidPositions = boids.Select(boid => boid.position).ToList();
        frameBoidVelocities = boids.Select(boid => boid.GetComponent<Rigidbody>().velocity).ToList();
        frameTargetPositions = targets.Select(target => target.position).ToList();
        frameAvoidPositions = avoids.Select(avoid => avoid.position).ToList();


        boidPositionsBuffer = new ComputeBuffer(numBoids, sizeof(float) * 3);
        boidVelocitiesBuffer = new ComputeBuffer(numBoids, sizeof(float) * 3);
        targetPositionsBuffer = new ComputeBuffer(numTargets, sizeof(float) * 3);
        avoidPositionsBuffer = new ComputeBuffer(numAvoids, sizeof(float) * 3);

        boidForcesBuffer = new ComputeBuffer(numBoids, sizeof(float) * 3);


        boidPositionsBuffer.SetData(frameBoidPositions.ToArray());
        boidVelocitiesBuffer.SetData(frameBoidVelocities.ToArray());
        targetPositionsBuffer.SetData(frameTargetPositions.ToArray());
        avoidPositionsBuffer.SetData(frameAvoidPositions.ToArray());


        BoidComputeShader.SetBuffer(0, "boidPositions", boidPositionsBuffer);
        BoidComputeShader.SetBuffer(0, "boidVelocities", boidVelocitiesBuffer);
        BoidComputeShader.SetBuffer(0, "targetPositions", targetPositionsBuffer);
        BoidComputeShader.SetBuffer(0, "avoidPositions", avoidPositionsBuffer);

        BoidComputeShader.SetBuffer(0, "boidForces", boidForcesBuffer);


        int threadGroups = Mathf.CeilToInt(numBoids / 256.0f);
        BoidComputeShader.Dispatch(0, threadGroups, 1, 1);

        Vector3[] forces = new Vector3[numBoids];
        boidForcesBuffer.GetData(forces);


        for (int boid = 0; boid < numBoids; boid++)
        {
            boidRigidbodies[boid].AddForce(forces[boid] * BoidFactor, ForceMode.Force);
        }
    }



    private void OnDestroy()
    {
        boidPositionsBuffer.Release();
        boidVelocitiesBuffer.Release();

        targetPositionsBuffer.Release();
        avoidPositionsBuffer.Release();

        boidForcesBuffer.Release();
    }
}
