
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Meta.XR.MRUtilityKit;



public class TorpedoManager : Singleton<TorpedoManager>
{
    private GameManager gameManager => GameManager.Instance;
    
    private Transform Player => PlayerManager.Instance.Player.transform;

    [SerializeField]
    private Transform torpedoPrefab;
    public Transform TorpedoPrefab => torpedoPrefab;

    [SerializeField]
    private Transform explosionEffect;
    public Transform ExplosionEffect => explosionEffect;

    private MRUK mruk => MRUK.Instance;
    private MRUKRoom currentMrukRoom = null;


    private struct Torpedo
    {
        public Vector3 position;
        public Vector3 velocity;

        public float fuseTimer;

        public GameManager.Alliance alliance;

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

    public List<Transform> Targets;

    private readonly List<Torpedo> torpedos = new();
    private readonly List<Torpedo> fallingTorpedos = new();
    private readonly HashSet<Torpedo> torpedosToExplode = new();



    /// <summary>
    /// Add a torpedo to the manager.
    /// </summary>
    /// <param name="torpedoTransform">The torpedo to add.</param>
    /// <param name="initialVelocity">The initial velocity of the torpedo.</param>
    /// <param name="torpedoAlliance">The alliance of the torpedo.</param>
    /// <returns>True if the torpedo was successfully added. Otherwise false.</returns>
    public bool AddTorpedo(Transform torpedoTransform, Vector3 initialVelocity, GameManager.Alliance torpedoAlliance)
    {
        if (transform == null) return false;

        TorpedoSettings torpedoSettings = torpedoAlliance switch
            {
                GameManager.Alliance.Player => PlayerTorpedoSettings,
                GameManager.Alliance.Enemy => EnemyTorpedoSettings,
                _ => new TorpedoSettings()
            };

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
    
    public bool AddFallingTorpedo(Transform torpedoTransform, Vector3 initialVelocity, GameManager.Alliance torpedoAlliance)
    {
        if (transform == null) return false;

        TorpedoSettings torpedoSettings = torpedoAlliance switch
        {
            GameManager.Alliance.Player => PlayerTorpedoSettings,
            GameManager.Alliance.Enemy => EnemyTorpedoSettings,
            _ => new TorpedoSettings()
        };

        Torpedo newTorpedo = new()
        {
            position = torpedoTransform.position,
            velocity = initialVelocity,
            fuseTimer = torpedoSettings.FuseTimer,
            alliance = torpedoAlliance,
            transform = torpedoTransform
        };

        fallingTorpedos.Add(newTorpedo);

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
        
        return ExplodeTorpedo(torpedo, torpedoTransform.position);
    }

    public bool ExplodeFallingTorpedo(Transform torpedoTransform)
    {
        if (torpedoTransform == null) return false;

        Torpedo torpedo = fallingTorpedos.Find(t => t.transform == torpedoTransform);
        
        return ExplodeTorpedo(torpedo, torpedoTransform.position);
    }



    public void ExplodeAllTorpedos(GameManager.Alliance? alliance = null)
    {
        foreach (Torpedo torpedo in torpedos)
        {
            if (alliance is not null && torpedo.alliance == alliance)
            {
                torpedosToExplode.Add(torpedo);
            }
        }
    }



    void Start()
    {
        gameManager.OnMruk += MrukRoomCreatedEvent;
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



    void OnDestroy()
    {
        if (GameManager.InstanceExists)
        {
            gameManager.OnMruk -= MrukRoomCreatedEvent;
        }
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

            // torpedo.fuseTimer -= Time.fixedDeltaTime;

            // Explode if fuse timer is up
            /*if (torpedo.fuseTimer <= 0.0f)
            {
                torpedosToExplode.Add(torpedo);
                continue;
            }*/

            bool withinExplodeRadius = false;
            Vector3 aimDirection = torpedo.velocity.normalized;
            Vector3? nearestTargetPos = torpedo.alliance switch
                {
                    GameManager.Alliance.Player => GetNearestTargetPosition(torpedo, targetPositions, out withinExplodeRadius),
                    GameManager.Alliance.Enemy =>  TargetPlayer(torpedo, out withinExplodeRadius),
                    _ => null
                };

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
            if (Physics.Raycast(torpedo.position, torpedo.velocity, out RaycastHit hit, torpedoSettings.ExplosionTriggerRadius))
            {
                torpedosToExplode.Add(torpedo);
                continue;
            }

            if (currentMrukRoom)
            {
                if (!currentMrukRoom.IsPositionInRoom(torpedo.position))
                {
                    torpedosToExplode.Add(torpedo);
                    continue;
                }
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
            ExplodeTorpedo(torpedo, torpedo.position);
        }

        torpedosToExplode.Clear();
    }



    private bool ExplodeTorpedo(Torpedo torpedo, Vector3 position)
    {
        gameManager.Explosion(torpedo.position, GetTorpedoSettings(torpedo.alliance).ExplosionPower, torpedo.alliance);

        if (ExplosionEffect != null)
        {
            var instantiatedEffect = Instantiate(ExplosionEffect, position, torpedo.transform.rotation);
            Destroy(instantiatedEffect.gameObject, instantiatedEffect.GetComponent<AudioSource>().clip.length);
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
        withinExplodeRadius = false;

        Vector3 playerPosition = Player.position;

        if (TorpedoCanSee(torpedo, playerPosition))
        {
            if (Vector3.Distance(torpedo.position, playerPosition) < GetTorpedoSettings(torpedo.alliance).ExplosionTriggerRadius)
            {
                withinExplodeRadius = true;
            }

            return playerPosition;
        }

        return null;
    }



    private bool TorpedoCanSee(Torpedo torpedo, Vector3 subPosition)
    {
        Vector3 direction = subPosition - torpedo.position;

        TorpedoSettings torpedoSettings = GetTorpedoSettings(torpedo.alliance);

        if (direction.magnitude > torpedoSettings.SearchRadius) return false;

        return Vector3.Angle(torpedo.velocity, direction) < torpedoSettings.SearchFov;
    }



    private TorpedoSettings GetTorpedoSettings(GameManager.Alliance torpedoAlliance)
    {
        return torpedoAlliance switch
        {
            GameManager.Alliance.Player => PlayerTorpedoSettings,
            GameManager.Alliance.Enemy => EnemyTorpedoSettings,
            _ => new TorpedoSettings(),
        };
    }


    private void MrukRoomCreatedEvent(object sender, GameManager.OnMrukCreatedArgs args)
    {
        currentMrukRoom = mruk.GetCurrentRoom();
    }
}
