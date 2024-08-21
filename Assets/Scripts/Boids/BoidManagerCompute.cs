using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BoidManager;

public class BoidManagerCompute : MonoBehaviour
{
    [SerializeField]
    // Scale the force of the boid manager
    public float BoidFactor;

    [SerializeField]
    // Scale the force of the boid manager
    public float MaxSpeed;

    [SerializeField]
    // Scale the force of the boid manager
    public float MinSpeed;


    [SerializeField]
    // How aggressivly the boids will avoid each other
    public float SeparationFactor;

    [SerializeField]
    // The distance boids will stay away from each other
    public float SeparationRadius;

    [SerializeField]
    // Will the boids avoid boids they can't see
    public bool SeparationUsesFov;


    [SerializeField]
    // How aggressivly boids will match the velocity of boids around them
    public float AlignmentFactor;

    [SerializeField]
    // The max distance for alignment
    public float AlignmentRadius;

    [SerializeField]
    // Will the boids align with boids they can't see
    public bool AlignmentUsesFov;


    [SerializeField]
    // How aggressivly boids will stick together
    public float CohesionFactor;

    [SerializeField]
    // The max distance for sticking together
    public float CohesionRadius;

    [SerializeField]
    // Will the boids go to boids they can't see
    public bool CohesionUsesFov;


    [SerializeField]
    // How aggressivly boids will move towards the target
    public float TargetFactor;

    [SerializeField]
    // The max distance for targeting
    public float TargetRadius;

    [SerializeField]
    // Will the boids go to targets they can't see
    public bool TargetUsesFov;

    [SerializeField]
    // Boid target mode
    public BoidRuleMode targetMode;

    [SerializeField]
    // The targets boids will move towards
    public List<Transform> targets;


    [SerializeField]
    // How aggressivly boids will avoid avoids
    public float AvoidFactor;

    [SerializeField]
    // The max distance for avoiding
    public float AvoidRadius;

    [SerializeField]
    // Will the boids avoid avoids they can't see
    public bool AvoidUsesFov;

    [SerializeField]
    // Boid avoid mode
    public BoidRuleMode avoidMode;

    [SerializeField]
    // The things boids will move away from
    public List<Transform> avoids;


    [SerializeField]
    // Limit to how far boids can go
    public float boundingRadius;

    [SerializeField]
    // When the boids start turning back to not go too far
    public float boundingEffectDistance;


    [SerializeField]
    // The maximum force the boid manager will apply
    public float maxForce;

    [SerializeField]
    // The minimum force the boid manager will apply
    public float minForce;


    [SerializeField]
    // The range of angles boids will see other boids
    public float boidFov;


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



    //void Start()
    //{
    //    boids = new List<Transform>();
    //    boidRigidbodies = new List<Rigidbody>();
    //}



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


        BoidComputeShader.SetFloat("separationRadius", SeparationRadius);
        BoidComputeShader.SetFloat("alignmentRadius", AlignmentRadius);
        BoidComputeShader.SetFloat("cohesionRadius", CohesionRadius);
        BoidComputeShader.SetFloat("targetRadius", TargetRadius);
        BoidComputeShader.SetFloat("avoidRadius", AvoidRadius);


        BoidComputeShader.SetFloat("separationFactor", SeparationFactor);
        BoidComputeShader.SetFloat("alignmentFactor", AlignmentFactor);
        BoidComputeShader.SetFloat("cohesionFactor", CohesionFactor);
        BoidComputeShader.SetFloat("targetFactor", TargetFactor);
        BoidComputeShader.SetFloat("avoidFactor", AvoidFactor);


        BoidComputeShader.SetBool("separationFov", SeparationUsesFov);
        BoidComputeShader.SetBool("alignmentFov", AlignmentUsesFov);
        BoidComputeShader.SetBool("cohesionFov", CohesionUsesFov);
        BoidComputeShader.SetBool("targetFov", TargetUsesFov);
        BoidComputeShader.SetBool("avoidFov", AvoidUsesFov);

        BoidComputeShader.SetFloat("halfFovRad", 0.5f * boidFov * Mathf.Deg2Rad);


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
            Vector3 boundingForce = Vector3.zero;

            if (Physics.Raycast(boids[boid].position, boids[boid].GetComponent<Rigidbody>().velocity, out RaycastHit hit, boundingRadius))
            {
                boundingForce = Vector3.Reflect(hit.normal, frameBoidVelocities[boid]) * maxForce;
            }


            Vector3 boidApplyForce = Vector3.ClampMagnitude(forces[boid] + boundingForce, maxForce);


            if (boidApplyForce.sqrMagnitude < 0.0000001f)
            {
                boidApplyForce = minForce * frameBoidVelocities[boid].normalized;
            }
            else if (boidApplyForce.sqrMagnitude < minForce * minForce)
            {
                boidApplyForce = minForce * boidApplyForce.normalized;
            }

            //Debug.Log(forces[boid]);

            boidRigidbodies[boid].AddForce(BoidFactor * boidApplyForce, ForceMode.Force);

            Vector3 newVel = Vector3.ClampMagnitude(boidRigidbodies[boid].velocity, MaxSpeed);

            if (newVel.sqrMagnitude < MinSpeed * MinSpeed)
            {
                newVel = MinSpeed * newVel.normalized;
            }

            boidRigidbodies[boid].velocity = newVel;
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
