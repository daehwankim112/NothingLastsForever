using Meta.XR.MRUtilityKit;
using Meta.XR.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySubmarineSpawner : MonoBehaviour
{
    [SerializeField] private Transform enemySubmarinePrefab;
    [SerializeField] private int numberOfSubmarines = 5;
    [SerializeField] private int maxIterations = 1000;
    [SerializeField] private MRUKAnchor.SceneLabels labels = ~(MRUKAnchor.SceneLabels)0;
    [SerializeField] private float surfaceClearanceDistance = 0.3f;
    [SerializeField] private EnemySubmarinesManager enemySubmarinesManager;

    private float tempTime = 0.0f;

    void Start()
    {
        if (MRUK.Instance)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                for (int i = 0; i < numberOfSubmarines; i++)
                {
                    SpawnSubmarines(MRUK.Instance.GetCurrentRoom());
                }
            });
        }
    }
    public void SpawnSubmarines(MRUKRoom room)
    {
        Debug.Log("Spawning a submarine");
        if (room.HasAllLabels(MRUKAnchor.SceneLabels.DOOR_FRAME) || room.HasAllLabels(MRUKAnchor.SceneLabels.WINDOW_FRAME))
        {
            for (int j = 0; j < maxIterations; j++)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                MRUK.SurfaceType surfaceType = 0;
                surfaceType |= MRUK.SurfaceType.FACING_UP;
                surfaceType |= MRUK.SurfaceType.VERTICAL;
                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                if (room.GenerateRandomPositionOnSurface(surfaceType, 0.1f, LabelFilter.Included(labels), out var pos, out var normal))
                {
                    spawnPosition = pos - normal * surfaceClearanceDistance;
                    spawnNormal = normal;
                    var center = spawnPosition;
                    if (room.IsPositionInRoom(center))
                    {
                        continue;
                    }
                    if (room.IsPositionInSceneVolume(center))
                    {
                        continue;
                    }
                    if (room.Raycast(new Ray(pos, normal), surfaceClearanceDistance, out _))
                    {
                        continue;
                    }
                }

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                if (enemySubmarinePrefab.gameObject.scene.path == null && spawnPosition != Vector3.zero)
                {
                    Transform instantiatedEnemySubmarinePrefab = Instantiate(enemySubmarinePrefab, spawnPosition, spawnRotation, transform);
                    enemySubmarinesManager.AddToEnemySubmarinesList(instantiatedEnemySubmarinePrefab);
                    instantiatedEnemySubmarinePrefab.GetComponent<EnemySubmarineController>().SetState(EnemySubmarineController.SubmarineState.GETINROOM);
                    instantiatedEnemySubmarinePrefab.GetComponent<Inventory>().Initialize(1, 1, 1, 1);
                    return;
                }
                else
                {
                    enemySubmarinePrefab.transform.position = spawnPosition;
                    enemySubmarinePrefab.transform.rotation = spawnRotation;
                    return;
                }
                break;
            }
        }
        else
        {
            Debug.Log($"Following labels do not exist in your room {labels.ToString()}");
        }
    }

    private void Update()
    {
        tempTime += Time.deltaTime;
        if (tempTime > 6.0f)
        {
            tempTime = 0.0f;
            SpawnSubmarines(MRUK.Instance.GetCurrentRoom());
        }
    }
}
