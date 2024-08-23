
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TorpedoManager : MonoBehaviour
{

    private struct Torpedo
    {
        public Vector3 position;
        public Vector3 velocity;

        public float fuseTimer;

        public Explosions.ExplosionAlliance alliance;

        public Transform transform;
    }


    [System.Serializable]
    public struct TorpedoSettings
    {
        public float EnginePower;

        public float FuseTimer;
        public float ExplosionPower;
        public float ExplosionTriggerRadius;

        public float SearchFov;
        public float SearchRadius;
    }

    public TorpedoSettings PlayerTorpedoSettings;
    public TorpedoSettings EnemyTorpedoSettings;

    public event Explosions.ExplosionEvent OnTorpedoExploded;

    public List<Transform> Targets;
    public Transform Player;

    private readonly List<Torpedo> torpedos = new();
    private readonly HashSet<Torpedo> torpedosToExplode = new();


    public GameObject ExplosionEffect;



    /// <summary>
    /// Add a torpedo to the manager.
    /// </summary>
    /// <param name="torpedoTransform">The torpedo to add.</param>
    /// <param name="initialVelocity">The initial velocity of the torpedo.</param>
    /// <param name="torpedoAlliance">The alliance of the torpedo.</param>
    /// <returns>True if the torpedo was successfully added. Otherwise false.</returns>
    public bool AddTorpedo(Transform torpedoTransform, Vector3 initialVelocity, Explosions.ExplosionAlliance torpedoAlliance)
    {
        if (transform == null) return false;

        TorpedoSettings torpedoSettings = (torpedoAlliance == Explosions.ExplosionAlliance.Player) ? PlayerTorpedoSettings : EnemyTorpedoSettings;

        Torpedo newTorpedo = new()
        {
            position = torpedoTransform.position,
            velocity = initialVelocity,
            fuseTimer = torpedoSettings.FuseTimer,
            alliance = torpedoAlliance,
            transform = torpedoTransform
        };

        torpedos.Add(newTorpedo);

        return true;
    }



    /// <summary>
    /// Destroy a torpedo. Does not trigger an explosion.
    /// </summary>
    /// <param name="torpedo">The torpedo to destroy.</param>
    /// <returns>True if the torpedo was successfully destroyed, false otherwise.</returns>
    public bool DestroyTorpedo(Transform torpedo)
    {
        if (torpedo == null) return false;

        Destroy(torpedo.gameObject);

        return torpedos.Remove(torpedos.Find(t => t.transform == torpedo));
    }



    /// <summary>
    /// Explode a torpedo.
    /// </summary>
    /// <param name="torpedo">The torpedo to explode.</param>
    /// <returns>True if the torpedo was successfully exploded, false otherwise.</returns>
    public bool ExplodeTorpedo(Transform torpedoTransform)
    {
        if (torpedoTransform == null) return false;

        Torpedo torpedo = torpedos.Find(t => t.transform == torpedoTransform);

        return ExplodeTorpedo(torpedo);
    }



    public void ExplodeAllTorpedos(Explosions.ExplosionAlliance? alliance = null)
    {
        foreach (Torpedo torpedo in torpedos)
        {
            if (alliance is not null && torpedo.alliance == alliance)
            {
                torpedosToExplode.Add(torpedo);
            }
        }
    }



    void FixedUpdate()
    {
        if (torpedos.Count == 0) return;

        UpdateTorpedos();
    }



    void LateUpdate()
    {
        if (torpedosToExplode.Count == 0) return;

        ExplodeTorpedos();
    }



    private void UpdateTorpedos()
    {
        int numTorpedos = torpedos.Count;

        Vector3[] targetPositions = null;
        if (Targets is not null && Targets.Count > 0)
        {
            targetPositions = Targets.Select(target => target.position).ToArray();
        }


        for (int torpedoIndex = 0; torpedoIndex < numTorpedos; torpedoIndex++)
        {
            Torpedo torpedo = torpedos[torpedoIndex];

            TorpedoSettings torpedoSettings = GetTorpedoSettings(torpedo.alliance);

            torpedo.fuseTimer -= Time.fixedDeltaTime;

            // Explode if fuse timer is up
            if (torpedo.fuseTimer <= 0.0f)
            {
                torpedosToExplode.Add(torpedo);
                continue;
            }

            bool withinExplodeRadius = false;
            Vector3 aimDirection = torpedo.velocity.normalized;
            Vector3? nearestTargetPos = (torpedo.alliance == Explosions.ExplosionAlliance.Enemy)
                                      ? TargetPlayer(torpedo, out withinExplodeRadius)
                                      : GetNearestTargetPosition(torpedo, targetPositions, out withinExplodeRadius);

            if (nearestTargetPos is not null)
            {
                // Explode if within explode trigger radius
                if (withinExplodeRadius)
                {
                    torpedosToExplode.Add(torpedo);
                    continue;
                }

                aimDirection = ((Vector3)nearestTargetPos - torpedo.position).normalized;
            }

            // Explode if going to hit something
            if (Physics.Raycast(torpedo.position, aimDirection, out RaycastHit hit, torpedoSettings.SearchRadius))
            {
                torpedosToExplode.Add(torpedo);
                continue;
            }

            // Update torpedo position, velocity, and rotation
            torpedo.velocity += Time.fixedDeltaTime * torpedoSettings.EnginePower * aimDirection;
            torpedo.position += Time.fixedDeltaTime * torpedo.velocity;
            torpedo.transform.SetPositionAndRotation(torpedo.position, Quaternion.LookRotation(torpedo.velocity));
            torpedos[torpedoIndex] = torpedo;
        }
    }



    private void ExplodeTorpedos()
    {
        foreach (Torpedo torpedo in torpedosToExplode)
        {
            ExplodeTorpedo(torpedo);
        }

        torpedosToExplode.Clear();
    }



    private bool ExplodeTorpedo(Torpedo torpedo)
    {
        OnTorpedoExploded?.Invoke(torpedo.position, GetTorpedoSettings(torpedo.alliance).ExplosionPower, torpedo.alliance);

        if (ExplosionEffect != null)
        {
            Instantiate(ExplosionEffect, torpedo.position, torpedo.transform.rotation);
        }

        return DestroyTorpedo(torpedo.transform);
    }



    private Vector3? GetNearestTargetPosition(Torpedo torpedo, Vector3[] targetPositions, out bool withinExplodeRadius)
    {
        withinExplodeRadius = false;

        if (targetPositions is null) return null;

        TorpedoSettings torpedoSettings = GetTorpedoSettings(torpedo.alliance);

        float minDistance = torpedoSettings.SearchRadius;
        Vector3? nearestSubPosition = null;

        foreach (Vector3 targetPosition in targetPositions)
        {
            float distance = Vector3.Distance(torpedo.position, targetPosition);

            if (distance < minDistance && TorpedoCanSee(torpedo, targetPosition))
            {
                minDistance = distance;
                nearestSubPosition = targetPosition;
            }
        }

        if (minDistance < torpedoSettings.ExplosionTriggerRadius)
        {
            withinExplodeRadius = true;
        }

        return nearestSubPosition;
    }



    private Vector3? TargetPlayer(Torpedo torpedo, out bool withinExplodeRadius)
    {
        Vector3? playerPosition = null;
        withinExplodeRadius = false;

        if (TorpedoCanSee(torpedo, Player.position))
        {
            playerPosition = Player.position;

            if (Vector3.Distance(torpedo.position, playerPosition.Value) < GetTorpedoSettings(torpedo.alliance).ExplosionTriggerRadius)
            {
                withinExplodeRadius = true;
            }
        }

        return playerPosition;
    }



    private bool TorpedoCanSee(Torpedo torpedo, Vector3 subPosition)
    {
        Vector3 direction = subPosition - torpedo.position;

        TorpedoSettings torpedoSettings = GetTorpedoSettings(torpedo.alliance);

        if (direction.magnitude > torpedoSettings.SearchRadius) return false;

        return Vector3.Angle(torpedo.velocity, direction) < torpedoSettings.SearchFov;
    }



    private TorpedoSettings GetTorpedoSettings(Explosions.ExplosionAlliance torpedoAlliance)
    {
        return (torpedoAlliance == Explosions.ExplosionAlliance.Player) ? PlayerTorpedoSettings : EnemyTorpedoSettings;
    }
}
