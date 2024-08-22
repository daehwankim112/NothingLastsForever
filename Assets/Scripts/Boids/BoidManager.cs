
using System.Linq;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This is an implementation of the Boids algorithm introduced by Craig Reynolds in 1987.
/// 
/// The algorithm simulates flocking behavior.
/// 
/// This implementation allows for the the separation, alignment, cohesion, and target paramaters to be set during runtime.
/// 
/// The boids and target can also be changed at runtime.
/// </summary>
public class BoidManager : MonoBehaviour
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


    /// <summary>
    /// What boids
    /// 
    /// Closest: Each boid will move toward the target closest to it
    /// 
    /// Average: Each boid will move toward the average position of all targets
    /// </summary>
    public enum BoidRuleMode
    {
        Closest,
        Average
    }



    void FixedUpdate()
    {
        UpdateBoids();
    }


    private List<int> frameBoids;
    private List<Vector3> frameBoidPositions;
    private List<Vector3> frameBoidVelocities;
    private List<Vector3> frameTargetPositions;
    private List<Vector3> frameAvoidPositions;



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



    /// <summary>
    /// Calculate the separation force felt by a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the separation force of.
    /// </param>
    /// <returns>
    /// The separation force felt by the boid.
    /// </returns>
    private Vector3 CalculateSeparationForce(int boid)
    {
        Vector3 boidPosition = frameBoidPositions[boid];
        Vector3 boidVelocity = frameBoidPositions[boid];

        List<int> validBoids = FilterPositions(frameBoids, frameBoidPositions, boid, SeparationUsesFov, SeparationDistance);
        Vector3 separationPosition = CalculateWeightedAveragePosition(validBoids, frameBoidPositions, boidPosition, SeparationDistance);

        Vector3 separationForce = boidPosition - separationPosition;

        return separationForce;
    }



    /// <summary>
    /// Calculate the alignment force felt by a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the alignment force of.
    /// </param>
    /// <returns>
    /// The alignment force felt by the boid.
    /// </returns>
    private Vector3 CalculateAlignmentForce(int boid)
    {
        Vector3 boidPosition = frameBoidPositions[boid];
        Vector3 boidVelocity = frameBoidPositions[boid];

        Vector3 averageNearbyVelocity = Vector3.zero;

        float weightSum = 0.0f;

        List<int> validBoids = FilterPositions(frameBoids, frameBoidPositions, boid, AlignmentUsesFov, AlignmentDistance);

        foreach (int validBoid in validBoids)
        {
            float distance = Vector3.Distance(frameBoidPositions[validBoid], boidPosition);

            float weight = 1.0f - (distance / AlignmentDistance);

            averageNearbyVelocity += weight * frameBoidVelocities[validBoid];

            weightSum += weight;
        }

        if (weightSum < 0.01f) return Vector3.zero;

        averageNearbyVelocity /= weightSum;

        Vector3 alignmentForce = averageNearbyVelocity - frameBoidVelocities[boid];

        return alignmentForce;
    }



    /// <summary>
    /// Calculate the cohesion force felt by a boid.
    /// The force pushing the boid towards nearby boids.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the cohesion force of.
    /// </param>
    /// <returns>
    /// The cohesion force felt by the boid.
    /// </returns>
    private Vector3 CalculateCohesionForce(int boid)
    {
        Vector3 boidPosition = frameBoidPositions[boid];
        Vector3 boidVelocity = frameBoidPositions[boid];

        List<int> validBoids = FilterPositions(frameBoids, frameBoidPositions, boid, CohesionUsesFov, CohesionDistance);
        Vector3 cohesionPosition = CalculateWeightedAveragePosition(validBoids, frameBoidPositions, boidPosition, CohesionDistance);

        Vector3 cohesionForce = cohesionPosition - boidPosition;

        return cohesionForce;
    }



    /// <summary>
    /// Calculate the force directing a boid towards the target.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the target force of.
    /// </param>
    /// <returns>
    /// The target force felt by the boid.
    /// </returns>
    private Vector3 CalculateTargetForce(int boid)
    {
        Vector3 targetPosition;
        Vector3 boidPosition = frameBoidPositions[boid];

        List<int> validTargets = FilterPositions(Enumerable.Range(0, targets.Count).ToList(), frameTargetPositions, boid, TargetUsesFov, MaxTargetDistance);

        switch (targetMode)
        {
            case BoidRuleMode.Average:
                targetPosition = CalculateWeightedAveragePosition(validTargets, frameTargetPositions, boidPosition, MaxTargetDistance);
                break;

            case BoidRuleMode.Closest:
                targetPosition = GetNearestPosition(validTargets, frameTargetPositions, boidPosition);
                break;

            default:
                return Vector3.zero;
        }

        Vector3 targetForce = (targetPosition - boidPosition).normalized;

        return targetForce;
    }



    /// <summary>
    /// Calculate the force directing a boid away from an object to avoid.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the avoidant force of.
    /// </param>
    /// <returns>
    /// The avoidant force felt by the boid.
    /// </returns>
    private Vector3 CalculateAvoidForce(int boid)
    {
        Vector3 avoidPosition;
        Vector3 boidPosition = frameBoidPositions[boid];

        List<int> validAvoids = FilterPositions(Enumerable.Range(0, avoids.Count).ToList(), frameAvoidPositions, boid, AvoidUsesFov, MaxAvoidDistance);

        switch (avoidMode)
        {
            case BoidRuleMode.Average:
                avoidPosition = CalculateWeightedAveragePosition(validAvoids, frameAvoidPositions, boidPosition, MaxAvoidDistance);
                break;

            case BoidRuleMode.Closest:
                avoidPosition = GetNearestPosition(validAvoids, frameAvoidPositions, boidPosition);
                break;

            default:
                return Vector3.zero;
        }

        Vector3 avoidForce = (boidPosition - avoidPosition).normalized;

        return avoidForce;
    }



    /// <summary>
    /// Calculate the force keeping a boid from going too far
    /// </summary>
    /// <param name="boid">
    /// The boid to bound
    /// </param>
    /// <returns>
    /// The bounding force
    /// </returns>
    private Vector3 CalculateBoundingForce(int boid)
    {
        Vector3 boidPosition = frameBoidPositions[boid];

        if (boidPosition.sqrMagnitude > boundingEffectDistance * boundingEffectDistance)
        {
            return -boidPosition / boundingDistance;
        }

        return Vector3.zero;
    }



    /// <summary>
    /// Calculate and apply the boid forces to a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to apply forces to.
    /// </param>
    private void ApplyForcesToBoid(int boid)
    {
        Vector3 separtationForce = CalculateSeparationForce(boid);
        Vector3 alignmentForce = CalculateAlignmentForce(boid);
        Vector3 cohesionForce = CalculateCohesionForce(boid);
        Vector3 targetForce = CalculateTargetForce(boid);
        Vector3 avoidForce = CalculateAvoidForce(boid);
        Vector3 boundingForce = CalculateBoundingForce(boid);

        Vector3 totalForce = (SeparationFactor * separtationForce)
                           + (AlignmentFactor * alignmentForce)
                           + (CohesionFactor * cohesionForce)
                           + (TargetFactor * targetForce)
                           + (AvoidFactor * avoidForce)
                           + (1000.0f * boundingForce);

        // Bound the force magnitude above
        Vector3 appliedForce = Vector3.ClampMagnitude(BoidFactor * totalForce, maxForce);

        Rigidbody boidRigidbody = boidRigidbodies[boid];

        // Bound the force magnitude below
        if (appliedForce.sqrMagnitude < 0.001f)
        {
            appliedForce = minForce * boidRigidbody.velocity.normalized;
        }
        else if (appliedForce.sqrMagnitude < minForce * minForce)
        {
            appliedForce = minForce * appliedForce.normalized;
        }

        boidRigidbody.AddForce(appliedForce, ForceMode.Force);
    }



    /// <summary>
    /// Calculate and apply boid forces to all boids.
    /// </summary>
    private void UpdateBoids()
    {
        // Fill frame lists
        int numBoids = boids.Count;

        frameBoids = Enumerable.Range(0, numBoids).ToList();
        frameBoidPositions = boids.Select(boid => boid.position).ToList();
        frameBoidVelocities = boids.Select(boid => boid.GetComponent<Rigidbody>().velocity).ToList();
        frameTargetPositions = targets.Select(target => target.position).ToList();
        frameAvoidPositions = avoids.Select(avoid => avoid.position).ToList();


        for (int boid = 0; boid < numBoids; boid++)
        {
            ApplyForcesToBoid(boid);
        }
    }



    /// <summary>
    /// Calculate the average position relative to a boid.
    /// 
    /// The weight is linear inverse of distance, 0 at maxDistance, 1 at 0
    /// </summary>
    /// <param name="indicies">
    /// The indicies in the positions list to get the average position of
    /// </param>
    /// <param name="positions">
    /// A list of positions
    /// </param>
    /// <param name="boidPosition">
    /// Boid position
    /// </param>
    /// <param name="maxDistance">
    /// Maximum distance to consider
    /// </param>
    /// <returns>
    /// The weighted average of distance.
    /// </returns>
    private Vector3 CalculateWeightedAveragePosition(List<int> indicies, List<Vector3> positions, Vector3 boidPosition, float maxDistance)
    {
        Vector3 averageNearbyPositon = Vector3.zero;

        float weightSum = 0.0f;

        foreach (int index in indicies)
        {
            Vector3 position = positions[index];

            float distance = (boidPosition - position).magnitude;

            if (distance > maxDistance) continue;

            // Map 0 - maxDistance to 1 - 0
            float weight = 1.0f - (distance / maxDistance);

            averageNearbyPositon += weight * position;

            weightSum += weight;
        }

        if (weightSum < 0.01f) return boidPosition;

        averageNearbyPositon /= weightSum;

        return averageNearbyPositon;
    }



    private List<int> FilterPositions(List<int> indicies, List<Vector3> positions, int boid, bool useFov, float maxDistance)
    {
        float sqrMaxDistance = maxDistance * maxDistance;

        Vector3 filterBoidPosition = frameBoidPositions[boid];
        Vector3 filterBoidForward = frameBoidVelocities[boid];

        return indicies.Where(index => (filterBoidPosition - positions[index]).sqrMagnitude < sqrMaxDistance
                                    && (!useFov || BoidVisible(filterBoidPosition, filterBoidForward, positions[index]))
                              )
                        .ToList();
    }



    private Vector3 GetNearestPosition(List<int> indicies, List<Vector3> positions, Vector3 boid)
    {
        if (positions == null || positions.Count == 0) return boid;

        return positions[indicies.OrderBy(index => (positions[index] - boid).sqrMagnitude)
                                 .FirstOrDefault()
                        ];
    }



    /// <summary>
    /// Check if a boid can see a location using boidFov.
    /// </summary>
    /// <param name="boidPosition">
    /// Position of the boid.
    /// </param>
    /// <param name="boidForward">
    /// Direction of the boid.
    /// </param>
    /// <param name="checkPosition">
    /// Position to check if the boid can see.
    /// </param>
    /// <returns>
    /// True if the boid can see checkPosition, false otherwise.
    /// </returns>
    private bool BoidVisible(Vector3 boidPosition, Vector3 boidForward, Vector3 checkPosition)
    {
        Vector3 otherDirection = (checkPosition - boidPosition);

        float angle = Vector3.Angle(boidForward, otherDirection);

        return angle < 0.5f * boidFov;
    }
}
