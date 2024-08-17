
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
    public float BoidFactor { get; set; } = 1.0f;

    [SerializeField]
    public float SeparationFactor { get; set; } = 1.0f;

    [SerializeField]
    public float AlignmentFactor { get; set; } = 1.0f;

    [SerializeField]
    public float CohesionFactor { get; set; } = 1.0f;

    [SerializeField]
    public float TargetFactor { get; set; } = 1.0f;

    [SerializeField]
    public Transform target { get; set; } = null;

    [SerializeField]
    public List<Transform> boids { get; set; } = new List<Transform>();



    void Start()
    {
        
    }



    void Update()
    {
        UpdateBoids();
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
        return Vector3.zero;
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
        return Vector3.zero;
    }



    /// <summary>
    /// Calculate the cohesion force felt by a boid.
    /// </summary>
    /// <param name="boid">
    /// The boid to calculate the cohesion force of.
    /// </param>
    /// <returns>
    /// The cohesion force felt by the boid.
    /// </returns>
    private Vector3 CalculateCohesionForce(Transform boid)
    {
        return Vector3.zero;
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

        return Vector3.zero;
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
        foreach (Transform boid in boids)
        {
            ApplyForcesToBoid(boid);
        }
    }
}
