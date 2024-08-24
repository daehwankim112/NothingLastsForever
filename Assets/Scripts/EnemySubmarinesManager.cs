using Meta.XR.MRUtilityKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;

/// <summary>
/// Enemy Submarine Manager manages the behaviour of the enemy submarines including: navigation, collision avoidance, and rotation.
/// Each submarine has a controller script with its state machine. The state machine indicates the state the submarine is in but submarine manager is responsible for physics of the submarine.
/// </summary>
public class EnemySubmarinesManager : MonoBehaviour
{
    UnityEvent onPlayerNotEchoingForSomeTime = new UnityEvent();

    [SerializeField] private SubmarinesTuneParameter submarinesTuneParameter;
    [HideInInspector] public List<Transform> enemySubmarines = new List<Transform>();
    [HideInInspector] public Transform playerPingLocation; // To be replced with PlayerPingLocation in the Game Manager. 8/24/2024 David Kim
    [SerializeField] private Transform OVRRigMainCamera;
    [SerializeField] private float timeBeforeSubmarinesStartEchoing = 10f;
    [SerializeField] private float lastTimeEchod = 0; // To be replaced with lastTimeEchoed in the Game Manager. 8/24/2024 David Kim
    [SerializeField] private float rangeOfSonarPingNeighbours = 2f;
    private Transform centreOfFloor;
    private Transform centreOfCeiling;
    private MRUKRoom room;

    // Parameters from Submarines Tune Parameter Scriptable Object
    private float enemySubmarineMaxSpeed;
    private float towardTheTargetWeight;
    private float rotateAroundTheTargetWeight;
    private float avoidCollisionWeight;
    private float collisionTestDistance;
    private float rotationEuqalibriumDistance;
    private float testDistance;
    private float testRadius;
    private int numberOfTest;
    private List<Vector3> towardTheTarget = new List<Vector3>();
    private List<Vector3> rotateAroundTheTarget = new List<Vector3>();
    private List<Vector3> avoidCollision = new List<Vector3>();

    private Vector3 target;
    private List<Transform> neighbouringSubmarinesOnPursue = new List<Transform>();

    /// <summary>
    /// Initialize the Enemy Submarine Manager with the parameters from the Submarines Tune Parameter Scriptable Object.
    /// </summary>
    void Start()
    {
        // Initialize the parameters according to the scriptable object
        enemySubmarineMaxSpeed = submarinesTuneParameter.enemySubmarineMaxSpeed;
        towardTheTargetWeight = submarinesTuneParameter.towardTheTargetWeight;
        rotateAroundTheTargetWeight = submarinesTuneParameter.rotateAroundTheTargetWeight;
        avoidCollisionWeight = submarinesTuneParameter.avoidCollisionWeight;
        collisionTestDistance = submarinesTuneParameter.collisionTestDistance;
        rotationEuqalibriumDistance = submarinesTuneParameter.rotationEuqalibriumDistance;
        testDistance = submarinesTuneParameter.testDistance;
        testRadius = submarinesTuneParameter.testRadius;
        numberOfTest = submarinesTuneParameter.numberOfTest;
        // Deprecated way of initializing the submarines
        /*for (int i = 0; i < enemySubmarines.Count; i++)
        {
            float distance = (mainCamera.position - enemySubmarines[i].position).magnitude;
            Vector3 projectedVector = Vector3.ProjectOnPlane(enemySubmarines[i].position - mainCamera.position, new Vector3(0, 1, 0));
            towardThePlayer.Add(enemySubmarines[i].position - mainCamera.position);
            rotateAroundThePlayer.Add(rotationYby90.MultiplyVector(new Vector4(projectedVector.x, projectedVector.y, projectedVector.z, 1)).normalized * 1/distance);
            avoidCollision.Add(Vector3.zero);
        }*/

        GameObject emptyGOForFloor = new GameObject();
        emptyGOForFloor.name = "CentreOfFloor";
        GameObject emptyGOForCeiling = new GameObject();
        emptyGOForCeiling.name = "CentreOfCeiling";
        centreOfFloor = emptyGOForFloor.transform;
        centreOfCeiling = emptyGOForCeiling.transform;

        if (MRUK.Instance)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                Initialize(MRUK.Instance.GetCurrentRoom());
            });
        }

        onPlayerNotEchoingForSomeTime.AddListener(ClosestSubmarineToPingAndTransitionNeighboursToPursue);
    }

    

    /// <summary>
    /// Initialize the room
    /// </summary>
    /// <param name="r">MRUKRoom to set center anchors for FLOOR and CEILING</param>
    private void Initialize(MRUKRoom r)
    {
        if (r.HasAllLabels(MRUKAnchor.SceneLabels.FLOOR))
        {
            centreOfFloor.position = r.FloorAnchor.GetAnchorCenter();
        }
        if (r.HasAllLabels(MRUKAnchor.SceneLabels.CEILING))
        {
            centreOfCeiling.position = r.CeilingAnchor.GetAnchorCenter();
        }
        room = r;
    }

    /// <summary>
    /// public room initialize function
    /// </summary>
    public void Initialize()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(() =>
        {
            Initialize(MRUK.Instance.GetCurrentRoom());
        });
    }

    private void Update()
    {
        lastTimeEchod += Time.deltaTime; // To be replaced with lastTimeEchoed in the Game Manager. 8/24/2024 David Kim
    }

    void FixedUpdate()
    {
        // Debug.Log("enemySubmarines count: " + enemySubmarines.Count);
        for (int i = 0; i < enemySubmarines.Count; i++)
        {
            switch(enemySubmarines[i].GetComponent<EnemySubmarineController>().GetState())
            {
                case EnemySubmarineController.SubmarineState.GETINROOM:
                    GetInRoom(i);
                    break;
                case EnemySubmarineController.SubmarineState.ROTATEAROUNDCENTRE:
                    RotateAroundCentre(i);
                    break;
                case EnemySubmarineController.SubmarineState.SONARPING:
                    SonarPing(i);
                    break;
                /*case EnemySubmarineController.SubmarineState.FIRETORPEDO:
                    FireTorpedo(i);
                    break;
                case EnemySubmarineController.SubmarineState.APPROACHPLAYER:
                    ApproachPlayer(i);
                    break;
                case EnemySubmarineController.SubmarineState.CHASEPLAYER:
                    ChasePlayer(i);
                    break;
                case EnemySubmarineController.SubmarineState.EXPLODES:
                    Explodes(i);
                    break;*/
            }
            /*if (enemySubmarines[i].GetComponent<EnemySubmarineController>().GetState() == EnemySubmarineController.SubmarineState.GETINROOM)
            {
                Vector3 resultingDirection;
                LayerMask layerMask = LayerMask.GetMask("Nothing");
                Debug.Log(i + " submarine is in GETINROOM state");
                towardThePlayer[i] = TowardTarget(OVRRigMainCamera.position, enemySubmarines[i].position);
                rotateAroundThePlayer[i] = RotateTarget(OVRRigMainCamera.position, enemySubmarines[i].position);
                avoidCollision[i] = AvoidCollision(enemySubmarines[i].position, enemySubmarines[i].forward, layerMask);
                if (room.IsPositionInRoom(enemySubmarines[i].position))
                {
                    enemySubmarines[i].GetComponent<EnemySubmarineController>().SetState(EnemySubmarineController.SubmarineState.ROTATEAROUNDCENTRE);
                    Debug.Log(i + " submarine is in ROTATEAROUNDCENTRE state");
                }
            }*/
            // Calculate the direction of the submarine towards the player
            /// towardThePlayer[i] = OVRRigMainCamera.position - enemySubmarines[i].position;
            /// float distance = towardThePlayer[i].magnitude;
            // towardThePlayer[i] = Mathf.Pow(distance * rotationEuqalibriumDistance, 2) * towardThePlayer[i].normalized;

            //towardThePlayer[i] = TowardTarget(OVRRigMainCamera.position, enemySubmarines[i].position);

            // Calculate the direction of the submarine to rotate around the player
            /// Vector3 projectedVector = Vector3.ProjectOnPlane(enemySubmarines[i].position - OVRRigMainCamera.position, new Vector3(0, 1, 0));
            /// rotateAroundThePlayer[i] = rotationYby90.MultiplyVector(new Vector4(projectedVector.x, projectedVector.y, projectedVector.z, 1)).normalized * 1/distance;

            // rotateAroundThePlayer[i] = RotateTarget(OVRRigMainCamera.position, enemySubmarines[i].position);

            // Calculate the direction of the submarine to avoid collision
            /// float closestDistance = 0;
            /// int numberOfTest = 18;
            /// int closestTestIndex = 0;
            /// Vector3 closestDistanceVector = Vector3.zero;

            Vector3 direction;
            
            /// if (!Physics.Raycast(enemySubmarines[i].position, enemySubmarines[i].forward, out RaycastHit hit, collisionTestDistance))
            /// {
            ///     direction = (towardThePlayer[i] * towardThePlayerWeight + rotateAroundThePlayer[i] * rotateAroundThePlayerWeight + avoidCollision[i] * avoidCollisionWeight).normalized;
            /// }
            /// else
            /// {
            ///     closestDistance = hit.distance;
            ///     for (int j = 0; j < numberOfTest; j++)
            ///     {
            ///         if (Physics.Raycast(enemySubmarines[i].position, (testDistance * enemySubmarines[i].forward + testRadius * (Mathf.Cos(Mathf.PI/numberOfTest) * new Vector3(1,0,0) + Mathf.Sin(Mathf.PI/numberOfTest) * new Vector3(0,1,0))).normalized, out hit, collisionTestDistance))
            ///         {
            ///             if (hit.distance < closestDistance)
            ///             {
            ///                 closestDistance = hit.distance;
            ///                 closestTestIndex = j;
            ///             }
            ///         }
            ///     }
            ///     closestDistanceVector = (testDistance * enemySubmarines[closestTestIndex].forward + testRadius * (Mathf.Cos(Mathf.PI / numberOfTest) * new Vector3(1, 0, 0) + Mathf.Sin(Mathf.PI / numberOfTest) * new Vector3(0, 1, 0)));
            ///     avoidCollision[i] = closestDistanceVector.normalized;
            ///     direction = (towardThePlayer[i] * towardThePlayerWeight + rotateAroundThePlayer[i] * rotateAroundThePlayerWeight + avoidCollision[i] * avoidCollisionWeight).normalized;
            /// }
            /// avoidCollision[i] = AvoidCollision(enemySubmarines[i].position, enemySubmarines[i].forward);
            direction = (towardTheTarget[i] * towardTheTargetWeight + rotateAroundTheTarget[i] * rotateAroundTheTargetWeight + avoidCollision[i] * avoidCollisionWeight).normalized;

            Debug.DrawRay(enemySubmarines[i].position, towardTheTarget[i], Color.red);
            Debug.DrawRay(enemySubmarines[i].position, rotateAroundTheTarget[i], Color.green);
            Debug.DrawRay(enemySubmarines[i].position, avoidCollision[i], Color.cyan);
            Debug.DrawRay(enemySubmarines[i].position, direction, Color.white);
            Quaternion lookOnLook = Quaternion.LookRotation(direction);
            enemySubmarines[i].rotation = Quaternion.Slerp(enemySubmarines[i].rotation, lookOnLook, Time.deltaTime);
            // enemySubmarines[i].LookAt(enemySubmarines[i].position + direction);
            enemySubmarines[i].position += enemySubmarines[i].forward * enemySubmarineMaxSpeed;
            // Debug.Log($"index: {i}\nToward the player: {towardThePlayer[i]}\nRotate around the player: {rotateAroundThePlayer[i]}\nAvoid collision: {avoidCollision[i]}");
        }
    }

    
    /// <summary>
    /// "GETINROOM" state. The submarine will move towards the centre of room.
    /// </summary>
    /// <param name="i"></param>
    private void GetInRoom(int i)
    {
        Vector3 resultingDirection;
        LayerMask layerMask = LayerMask.GetMask("Nothing");
        Debug.Log(i + " submarine is in GETINROOM state");
        Debug.Log("centre of the floor: " + centreOfFloor.position);
        Debug.Log("centre of the ceiling: " + centreOfCeiling.position);
        towardTheTarget[i] = TowardTarget( (centreOfFloor.position + centreOfCeiling.position) / 2, enemySubmarines[i].position);
        rotateAroundTheTarget[i] = Vector3.zero;
        avoidCollision[i] = AvoidCollision(enemySubmarines[i].position, enemySubmarines[i].forward, layerMask);

        // Transition to "ROTATEAROUNDCENTRE" state if the submarine is in the room.
        if (room.IsPositionInRoom(enemySubmarines[i].position + (enemySubmarines[i].position - OVRRigMainCamera.position) / 10f))
        {
            enemySubmarines[i].GetComponent<EnemySubmarineController>().SetState(EnemySubmarineController.SubmarineState.ROTATEAROUNDCENTRE);
            Debug.Log(i + " submarine is in ROTATEAROUNDCENTRE state");
        }
    }
    /// <summary>
    /// "ROTATEAROUNDCENTRE" state. The submarine will rotate around the player.
    /// </summary>
    /// <param name="i"></param>
    private void RotateAroundCentre(int i)
    {
        Vector3 resultingDirection;
        LayerMask layerMask = LayerMask.GetMask("Default");
        Debug.Log(i + " submarine is in GETINROOM state");
        towardTheTarget[i] = TowardTarget(OVRRigMainCamera.position, enemySubmarines[i].position);
        rotateAroundTheTarget[i] = RotateTarget(OVRRigMainCamera.position, enemySubmarines[i].position);
        avoidCollision[i] = AvoidCollision(enemySubmarines[i].position, enemySubmarines[i].forward, layerMask);

        // if statements to check if the submarine is close to the player has not been echoing for some time.
        // If so, check the closest sub to the player and transition that submarine to "SONARPING" state.
        // Otherwise, check if the submarine is close to the "SONARPING" submarine and transition that submarine to
        // "FIRETORPEDO" state if it is close enough otherwise transition to "APPROACHPLAYER" state.
        // If player echos, reset the timer and transition to "FIRETORPEDO" state.
        if (lastTimeEchod > timeBeforeSubmarinesStartEchoing)
        {
            lastTimeEchod = 0f;
        }
    }


    private void SonarPing(int i)
    {
        playerPingLocation = OVRRigMainCamera;
        // play sonar ping sound once

        throw new NotImplementedException();
    }

    private Vector3 TowardTarget(Vector3 target, Vector3 position)
    {
        Vector3 targetVector = target - position;
        float distance = targetVector.magnitude;
        return Mathf.Pow(distance * rotationEuqalibriumDistance, 2) * targetVector.normalized;
    }

    private Vector3 RotateTarget(Vector3 target, Vector3 position)
    {
        Matrix4x4 rotationYby90 = new Matrix4x4(new Vector4(0f, 0f, 1f, 0f),
                                                    new Vector4(0f, 1f, 0f, 0f),
                                                    new Vector4(-1f, 0f, 0f, 0f),
                                                    new Vector4(0f, 0f, 0f, 1f));
        Vector3 projectedVector = Vector3.ProjectOnPlane(position - target, new Vector3(0, 1, 0));
        return rotationYby90.MultiplyVector(new Vector4(projectedVector.x, projectedVector.y, projectedVector.z, 1)).normalized;
    }

    private Vector3 AvoidCollision(Vector3 position, Vector3 forward, LayerMask layerMask)
    {
        float closestDistance = 0;
        int closestTestIndex = 0;
        Vector3 closestDistanceVector = Vector3.zero;

        if (!Physics.Raycast(position, forward, out RaycastHit initialHit, collisionTestDistance, layerMask))
        {
            return Vector3.zero;
        }
        else
        {
            closestDistance = initialHit.distance;
            for (int j = 0; j < numberOfTest; j++)
            {
                if (Physics.Raycast(position, (testDistance * forward + testRadius * (Mathf.Cos( (Mathf.PI / numberOfTest) * j) * new Vector3(1, 0, 0) + Mathf.Sin( (Mathf.PI / numberOfTest) * j) * new Vector3(0, 1, 0))).normalized, out RaycastHit FrontalHit, collisionTestDistance, layerMask))
                {
                    if (FrontalHit.distance < closestDistance)
                    {
                        closestDistance = FrontalHit.distance;
                        closestTestIndex = j;
                    }
                }
                if (Physics.Raycast(position, (testRadius * (Mathf.Cos((Mathf.PI / numberOfTest) * j) * new Vector3(1, 0, 0) + Mathf.Sin((Mathf.PI / numberOfTest) * j) * new Vector3(0, 1, 0))).normalized, out RaycastHit sideHit, collisionTestDistance, layerMask))
                {
                    if (sideHit.distance < closestDistance)
                    {
                        closestDistance = sideHit.distance;
                        closestTestIndex = j;
                    }
                }
            }
            closestDistanceVector = testDistance * forward + testRadius * (Mathf.Cos( (Mathf.PI / numberOfTest) * closestTestIndex) * new Vector3(1, 0, 0) + Mathf.Sin( (Mathf.PI / numberOfTest) * closestTestIndex) * new Vector3(0, 1, 0));
            return - closestDistanceVector.normalized * 1 / closestDistance;
        }
    }


    public void AddToEnemySubmarinesList(Transform submarine)
    {
        enemySubmarines.Add(submarine);
        towardTheTarget.Add(Vector3.zero);
        rotateAroundTheTarget.Add(Vector3.zero);
        avoidCollision.Add(Vector3.zero);
    }

    private void ClosestSubmarineToPingAndTransitionNeighboursToPursue()
    {
        float closestDistance = 0;
        int closestSubmarineIndex = 0;
        for (int i = 0; i < enemySubmarines.Count; i++)
        {
            if (Vector3.Distance(OVRRigMainCamera.position, enemySubmarines[i].position) < closestDistance)
            {
                closestDistance = Vector3.Distance(OVRRigMainCamera.position, enemySubmarines[i].position);
                closestSubmarineIndex = i;
            }
        }

        enemySubmarines[closestSubmarineIndex].GetComponent<EnemySubmarineController>().SetState(EnemySubmarineController.SubmarineState.SONARPING);

        for (int i = 0; i < enemySubmarines.Count; i++)
        {
            if (Vector3.Distance(enemySubmarines[closestSubmarineIndex].position, enemySubmarines[i].position) < rangeOfSonarPingNeighbours)
            {
                neighbouringSubmarinesOnPursue.Add(enemySubmarines[i]);
                enemySubmarines[i].GetComponent<EnemySubmarineController>().SetState(EnemySubmarineController.SubmarineState.APPROACHPLAYER);
            }
        }
    }
}
