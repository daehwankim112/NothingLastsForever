
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
    public float BoidFactor = 1.0f;

    [SerializeField]
    public float SeparationFactor = 1.0f;

    [SerializeField]
    public float SeparationDistance = 1.0f;

    [SerializeField]
    public float AlignmentFactor = 1.0f;

    [SerializeField]
    public float AlignmentDistance = 1.0f;

    [SerializeField]
    public float CohesionFactor = 1.0f;

    [SerializeField]
    public float CohesionDistance = 1.0f;

    [SerializeField]
    public float TargetFactor = 1.0f;

    [SerializeField]
    public Transform target = null;

    [SerializeField]
    public List<Transform> boids = new List<Transform>();



    private Vector3 boidsAveragePosition = Vector3.zero;

    private Vector3 boidsAverageVelocity = Vector3.zero;



    void Start()
    {
        
    }



    void Update()
    {
        UpdateBoids();
    }



    /// <summary>
    /// Add a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to add.
    /// </param>
    public void AddBoid(Transform boid)
    {
        boids.Add(boid);
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
        return boids.Remove(boid);
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
    private Vector3 CalculateSeparationForce(Transform boid)
    {
        Vector3 separationForce = Vector3.zero;

        foreach (Transform nearBoid in boids)
        {
            // A boid cannot separate from itself
            if (nearBoid == boid) continue;

            // Ignore boids that are too far away
            if ((boid.position - nearBoid.position).sqrMagnitude > SeparationDistance * SeparationDistance) continue;

            separationForce -= (nearBoid.position - boid.position);
        }

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
    private Vector3 CalculateAlignmentForce(Transform boid)
    {
        if (!boid.TryGetComponent<Rigidbody>(out Rigidbody boidRigidbody)) return Vector3.zero;


        Vector3 alignmentForce = Vector3.zero;

        Vector3 averageNearbyVelocity = Vector3.zero;

        int numNearbyBoids = 0;

        foreach (Transform nearBoid in boids)
        {
            // A boid cannot get closer to itself
            if (nearBoid == boid) continue;

            if (!nearBoid.TryGetComponent<Rigidbody>(out Rigidbody nearBoidRigidbody)) continue;

            // Ignore boids that are too far away
            if ((boid.position - nearBoid.position).sqrMagnitude > AlignmentDistance * AlignmentDistance) continue;

            averageNearbyVelocity += nearBoidRigidbody.velocity;

            numNearbyBoids++;
        }

        if (numNearbyBoids == 0) return Vector3.zero;

        averageNearbyVelocity /= numNearbyBoids;

        alignmentForce = averageNearbyVelocity - boidRigidbody.velocity;

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
    private Vector3 CalculateCohesionForce(Transform boid)
    {
        Vector3 cohesionForce = Vector3.zero;

        Vector3 averageNearbyPositon = Vector3.zero;

        int numNearbyBoids = 0;

        foreach (Transform nearBoid in boids)
        {
            // A boid cannot get closer to itself
            if (nearBoid == boid) continue;

            // Ignore boids that are too far away
            if ((boid.position - nearBoid.position).sqrMagnitude > CohesionDistance * CohesionDistance) continue;

            averageNearbyPositon += nearBoid.position;

            numNearbyBoids++;
        }

        if (numNearbyBoids == 0) return Vector3.zero;

        averageNearbyPositon /= numNearbyBoids;

        cohesionForce = averageNearbyPositon - boid.position;

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
    private Vector3 CalculateTargetForce(Transform boid)
    {
        if (target == null) return Vector3.zero;

        return (target.position - boid.position).normalized;
    }



    /// <summary>
    /// Calculate and apply the boid forces to a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to apply forces to.
    /// </param>
    private void ApplyForcesToBoid(Transform boid)
    {
        if (!boid.TryGetComponent<Rigidbody>(out Rigidbody boidRigidbody)) return;

        Vector3 separtationForce = CalculateSeparationForce(boid);
        Vector3 alignmentForce = CalculateAlignmentForce(boid);
        Vector3 cohesionForce = CalculateCohesionForce(boid);
        Vector3 targetForce = CalculateTargetForce(boid);

        Vector3 totalForce = (SeparationFactor * separtationForce)
                           + (AlignmentFactor * alignmentForce)
                           + (CohesionFactor * cohesionForce)
                           + (TargetFactor * targetForce);

        boidRigidbody.AddForce(BoidFactor * totalForce, ForceMode.Force);
    }



    /// <summary>
    /// Calculate and apply boid forces to all boids.
    /// </summary>
    private void UpdateBoids()
    {
        //boidsAveragePosition = CalculateBoidsAveragePosition();
        //boidsAverageVelocity = CalculateBoidsAverageVelocity();

        foreach (Transform boid in boids)
        {
            ApplyForcesToBoid(boid);
        }
    }



    /// <summary>
    /// Calculate the average position of all the boids.
    /// </summary>
    /// <returns>
    /// The average position of the boids.
    /// </returns>
    private Vector3 CalculateBoidsAveragePosition()
    {
        Vector3 center = Vector3.zero;

        foreach(Transform boid in boids)
        {
            center += boid.position;
        }

        center /= boids.Count;

        return center;
    }



    /// <summary>
    /// Calculate the average velocity of all the boids.
    /// </summary>
    /// <returns>
    /// The average velocity of the boids.
    /// </returns>
    private Vector3 CalculateBoidsAverageVelocity()
    {
        Vector3 center = Vector3.zero;

        foreach (Transform boid in boids)
        {
            if (!boid.TryGetComponent<Rigidbody>(out Rigidbody boidRigidbody)) continue;

            center += boidRigidbody.velocity;
        }

        center /= boids.Count;

        return center;
    }
}
