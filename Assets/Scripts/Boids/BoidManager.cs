
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using Meta.XR.MRUtilityKit;
using Meta.WitAi.TTS.Data;



public class BoidManager : Singleton<BoidManager>, IDifficultySensor
{
    private GameManager gameManager => GameManager.Instance;

    private Settings settings => gameManager.Settings;

    private MRUK mruk => MRUK.Instance;
    private MRUKRoom currentMrukRoom = null;



    private struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 AvoidCollisionDirection;
        public int FramesSinceLastCollisionCheck;
        public Transform transform;
    }



    private struct BufferBoid
    {
        public Vector3 position;
        public Vector3 velocity;

        public Vector3 force;
    }



    [System.Serializable]
    public struct BoidSettings
    {
        public float MaxSpeed;
        public float MinSpeed;
        public float MaxTurning;
        public float boidFov;

        public float SeparationFactor;
        public float SeparationRadius;
        public int SeparationUsesFov;

        public float AlignmentFactor;
        public float AlignmentRadius;
        public int AlignmentUsesFov;

        public float CohesionFactor;
        public float CohesionRadius;
        public int CohesionUsesFov;

        public float TargetFactor;
        public float TargetRadius;
        public int TargetUsesFov;

        public float AvoidFactor;
        public float AvoidRadius;
        public int AvoidUsesFov;
    }


    public float CollisionAvoidanceFactor;
    public float CollisionAvoidanceRadius;
    public int FramesBetweenCollisionChecks;
    private int collisionCheckStagger = 0;

    public float ExplosionPowerDeathCertainty;
    public float ExplosionPowerSurvivalCertainty;

    public int NumBoids => boids.Count;



    public BoidSettings boidSettings;
    public List<Transform> targets;
    public List<Transform> avoids;

    [SerializeField]
    private readonly List<Boid> boids = new();

    private readonly List<int> deadBoidIndicies = new();

    public ComputeShader BoidComputeShader;
    private ComputeBuffer settingsBuffer;
    private ComputeBuffer boidBuffer;
    private ComputeBuffer targetPositionsBuffer, avoidPositionsBuffer;

    private BufferBoid[] bufferBoids;


    private void Start()
    {
        gameManager.OnExplosion += OnTorpedoExploded;
        gameManager.OnMruk += MrukRoomCreatedEvent;
    }



    void FixedUpdate()
    {
        if (boids.Count == 0) return;

        FillBuffers();

        ComputeBoidShader();

        UpdateBoids();

        ReleaseBuffers();
    }



    private void LateUpdate()
    {
        RemoveDeadBoids();
    }



    private void OnDestroy()
    {
        if (GameManager.InstanceExists)
        {
            gameManager.OnExplosion -= OnTorpedoExploded;
            gameManager.OnMruk -= MrukRoomCreatedEvent;
        }
        ReleaseBuffers();
    }



    /// <summary>
    /// Adds a new boid to the boid manager.
    /// </summary>
    /// <param name="boidTransform">The transform of the boid to add.</param>
    /// <returns>True if the boid was successfully added, false otherwise.</returns>
    public bool AddBoid(Transform boidTransform, Vector3? initialVelocity = null)
    {
        if (boidTransform == null) return false;

        collisionCheckStagger++;

        Boid newBoid = new()
        {
            position = boidTransform.position,
            velocity = initialVelocity.GetValueOrDefault(Vector3.zero),
            AvoidCollisionDirection = Vector3.zero,
            FramesSinceLastCollisionCheck = collisionCheckStagger % FramesBetweenCollisionChecks,
            transform = boidTransform
        };

        boids.Add(newBoid);

        return true;
    }



    /// <summary>
    /// Removes a boid from the boid manager.
    /// </summary>
    /// <param name="destroyGameobject">Whether or not to destroy the gameobject associated with the boid.</param>
    /// <returns>True if the boid was successfully removed, false otherwise.</returns>
    public bool RemoveBoid(bool destroyGameobject = false)
    {
        return RemoveBoid(boids.Count - 1, destroyGameobject);
    }



    /// <summary>
    /// Removes a boid from the boid manager.
    /// </summary>
    /// <param name="boid">The transform of the boid to be removed.</param>
    /// <param name="destroyGameobject">Whether or not to destroy the gameobject associated with the boid.</param>
    /// <returns>True if the boid was successfully removed, false otherwise.</returns>
    public bool RemoveBoid(Transform boid, bool destroyGameobject = false)
    {
        return RemoveBoid(boids.FindIndex(b => b.transform == boid), destroyGameobject);
    }


    public float GetDifficulty()
    {
        return settings.BoidWeight * NumBoids;
    }



    /// <summary>
    /// Fills the buffers with boid settings, boid data, target positions, and avoid positions.
    /// </summary>
    private void FillBuffers()
    {
        int numTargets = targets.Count;
        int numAvoids = avoids.Count;


        boidBuffer = new ComputeBuffer(NumBoids, Marshal.SizeOf(typeof(BufferBoid)));
        settingsBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(BoidSettings)));


        BufferBoid[] bufferBoids = new BufferBoid[boids.Count];
        for (int i = 0; i < boids.Count; i++)
        {
            bufferBoids[i] = new BufferBoid
            {
                position = boids[i].position,
                velocity = boids[i].velocity,
                force = Vector3.zero
            };
        }


        boidBuffer.SetData(bufferBoids);
        settingsBuffer.SetData(new BoidSettings[] { boidSettings });

        if (numTargets > 0)
        {
            targetPositionsBuffer = new ComputeBuffer(numTargets, sizeof(float) * 3);
            targetPositionsBuffer.SetData(targets.Select(target => target.position).ToArray());
        }
        else
        {
            targetPositionsBuffer = new ComputeBuffer(1, sizeof(float) * 3);
            targetPositionsBuffer.SetData(new Vector3[1] { new(1e15f, 1e15f, 1e15f) });
        }

        if (numAvoids > 0)
        {
            avoidPositionsBuffer = new ComputeBuffer(numAvoids, sizeof(float) * 3);
            avoidPositionsBuffer.SetData(avoids.Select(avoid => avoid.position).ToArray());
        }
        else
        {
            avoidPositionsBuffer = new ComputeBuffer(1, sizeof(float) * 3);
            avoidPositionsBuffer.SetData(new Vector3[1] { new(1e15f, 1e15f, 1e15f) });
        }


        BoidComputeShader.SetBuffer(0, "boids", boidBuffer);
        BoidComputeShader.SetBuffer(0, "boidSettings", settingsBuffer);
        BoidComputeShader.SetBuffer(0, "targetPositions", targetPositionsBuffer);
        BoidComputeShader.SetBuffer(0, "avoidPositions", avoidPositionsBuffer);
    }



    private void ComputeBoidShader()
    {
        // Calculate the number of thread groups needed based on the number of boids
        int threadGroups = Mathf.CeilToInt((float)NumBoids / 256.0f);
        BoidComputeShader.Dispatch(0, threadGroups, 1, 1);

        // Get the data from the boid buffer
        bufferBoids = new BufferBoid[NumBoids];
        boidBuffer.GetData(bufferBoids);
    }



    /// <summary>
    /// Updates the behavior of the boids.
    /// </summary>
    private void UpdateBoids()
    {
        // Update each boid's position and velocity
        for (int boidIndex = 0; boidIndex < NumBoids; boidIndex++)
        {
            Boid boid = boids[boidIndex];

            // Calculate the force acting on the boid
            Vector3 force = bufferBoids[boidIndex].force;

            boid.FramesSinceLastCollisionCheck++;
            Vector3 avoidCollisionDirection = Vector3.zero;
            if (boid.FramesSinceLastCollisionCheck >= FramesBetweenCollisionChecks)
            {
                avoidCollisionDirection = FindSafePath(boid.position, boid.velocity);
                boid.AvoidCollisionDirection = avoidCollisionDirection;
                boid.FramesSinceLastCollisionCheck = 0;
            }

            force += CollisionAvoidanceFactor * boid.AvoidCollisionDirection;
            force = Vector3.ClampMagnitude(force, boidSettings.MaxTurning);

            // Update the velocity of the boid
            Vector3 velocity = boid.velocity;
            velocity += Time.fixedDeltaTime * force;

            // Calculate the speed of the boid
            float speed = velocity.magnitude;

            // Adjust the velocity based on the speed limits
            if (speed <= 0.00001f)
            {
                if (force.sqrMagnitude <= 0.00001f)
                {
                    force = boidSettings.MaxTurning * Random.onUnitSphere;
                }

                velocity = force;
            }
            else if (speed < boidSettings.MinSpeed)
            {
                velocity = (velocity / speed) * boidSettings.MinSpeed;
            }
            else if (speed > boidSettings.MaxSpeed)
            {
                velocity = (velocity / speed) * boidSettings.MaxSpeed;
            }

            // Update the position of the boid
            boid.velocity = velocity;
            boid.position += Time.fixedDeltaTime * velocity;
            boid.transform.position = boid.position;

            if (currentMrukRoom)
            {
                if (!currentMrukRoom.IsPositionInRoom(boid.position))
                {
                    currentMrukRoom.TryGetClosestSurfacePosition(boid.position, out Vector3 surfacePosition, out MRUKAnchor closestAnchor);

                    Vector3 normal = (surfacePosition - boid.position).normalized;
                    boid.velocity = Vector3.Reflect(boid.velocity, normal);

                    boid.position = surfacePosition + (0.02f * normal);
                }
            }

            boids[boidIndex] = boid;
        }
    }



    /// <summary>
    /// Finds a safe path for a boid to avoid collisions.
    /// </summary>
    /// <param name="position">The current position of the boid.</param>
    /// <param name="velocity">The current velocity of the boid.</param>
    /// <param name="numCircles">The number of circles to search for safe paths.</param>
    /// <param name="searchesPerCircle">The number of searches to perform per circle.</param>
    /// <returns>The safe path for the boid to follow.</returns>
    private Vector3 FindSafePath(Vector3 position, Vector3 velocity, int numCircles = 4, int searchesPerCircle = 4)
    {
        Vector3 safePath = Vector3.zero;

        // Check is forward path is safe
        if (!Physics.Raycast(position, velocity, out RaycastHit hit, CollisionAvoidanceRadius))
        {
            return safePath;
        }

        float farthestDistance = hit.distance;
        Vector3 forwardDirection = velocity.normalized;
        Vector3 orthogonalDirection = Vector3.Cross(forwardDirection, hit.normal);

        // Iterate through search vectors
        for (int i = numCircles; i > 1; i--)
        {
            // Start at 90 degrees, search forward, then start backwards and search to 90 degrees
            float searchAngle = (float)((i + (numCircles / 2)) % numCircles) * 180.0f / (float)numCircles;

            for (int j = 0; j < searchesPerCircle; j++)
            {
                // Search around the full circle
                float searchDirection = (float)j * 360.0f / (float)searchesPerCircle;

                Vector3 testDirection = Quaternion.AngleAxis(searchDirection, forwardDirection)
                                      * (Quaternion.AngleAxis(searchAngle, orthogonalDirection)
                                      * forwardDirection);

                if (Physics.Raycast(position, testDirection, out RaycastHit randomHit, CollisionAvoidanceRadius))
                {
                    if (Vector3.Dot(randomHit.normal, forwardDirection) <= 0 && randomHit.distance > farthestDistance)
                    {
                        farthestDistance = randomHit.distance;
                        safePath = testDirection;
                    }
                }
                else
                {
                    return testDirection;
                }
            }
        }

        return safePath;
    }



    /// <summary>
    /// Releases the buffers used by the BoidManagerCompute.
    /// </summary>
    private void ReleaseBuffers()
    {
        if (boidBuffer != null)
        {
            boidBuffer.Release();
            boidBuffer = null;
        }

        if (settingsBuffer != null)
        {
            settingsBuffer.Release();
            settingsBuffer = null;
        }

        if (targetPositionsBuffer != null)
        {
            targetPositionsBuffer.Release();
            targetPositionsBuffer = null;
        }

        if (avoidPositionsBuffer != null)
        {
            avoidPositionsBuffer.Release();
            avoidPositionsBuffer = null;
        }
    }



    private void RemoveDeadBoids()
    {
        foreach (int deadBoidIndex in deadBoidIndicies)
        {
            RemoveBoid(deadBoidIndex, true);
        }
    }



    private bool RemoveBoid(Boid boid, bool destroyGameobject = false)
    {
        int index = boids.FindIndex(b => b.Equals(boid));
        bool successful = index >= 0;

        return successful && RemoveBoid(index, destroyGameobject);
    }



    /// <summary>
    /// This is the ONLY function that will actually remove a boid from the boid manager.
    /// </summary>
    /// <param name="boidIndex"></param>
    /// <param name="destroyGameobject"></param>
    /// <returns></returns>
    private bool RemoveBoid(int boidIndex, bool destroyGameobject = false)
    {
        Boid boid = boids[boidIndex];

        boids.RemoveAt(boidIndex);

        if (destroyGameobject)
        {
            Destroy(boid.transform.gameObject);
        }

        return true;
    }



    private void OnTorpedoExploded(object sender, GameManager.OnExplosionArgs explosionArgs)
    {
        if (explosionArgs.ExplosionAlliance != GameManager.Alliance.Player) return;

        for (int boidIndex = 0; boidIndex < boids.Count; boidIndex++)
        {
            Boid boid = boids[boidIndex];

            float distance = Vector3.Distance(explosionArgs.Position, boid.position);

            float explosionPower = explosionArgs.Power / (distance * distance);

            boid.velocity += explosionPower * (boid.position - explosionArgs.Position).normalized;

            float survivalChance = Random.value;

            if (survivalChance < Mathf.InverseLerp(ExplosionPowerSurvivalCertainty, ExplosionPowerDeathCertainty, explosionPower))
            {
                deadBoidIndicies.Add(boidIndex);
            }

            boids[boidIndex] = boid;
        }
    }



    private void MrukRoomCreatedEvent(object sender, GameManager.OnMrukCreatedArgs args)
    {
        currentMrukRoom = mruk.GetCurrentRoom();
    }
}
